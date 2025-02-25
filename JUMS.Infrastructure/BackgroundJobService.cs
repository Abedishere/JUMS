using Hangfire;
using Microsoft.Extensions.Logging;
using JUMS.Domain.Interfaces;
using JUMS.Infrastructure.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JUMS.Infrastructure.BackgroundJobs
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly ISharedLocalizer _localizer;

        public BackgroundJobService(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            INotificationService notificationService,
            ILogger<BackgroundJobService> logger,
            ISharedLocalizer localizer)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _notificationService = notificationService;
            _logger = logger;
            _localizer = localizer;
        }

        public void RegisterRecurringJobs()
        {
            // Hourly job: Recalculate student grade averages
            RecurringJob.AddOrUpdate(
                "student-grade-recalculation",
                () => RecalculateStudentGradesAsync(),
                Cron.Hourly);

            _logger.LogInformation(_localizer["BackgroundJobs.GradeRecalculationScheduled"]);

            // Daily job at 9:00 AM: Notify about enrollment deadlines
            RecurringJob.AddOrUpdate(
                "enrollment-deadline-notification",
                () => SendEnrollmentDeadlineNotificationsAsync(),
                Cron.Daily(9));

            _logger.LogInformation(_localizer["BackgroundJobs.EnrollmentNotificationsScheduled"]);
        }

        public async Task RecalculateStudentGradesAsync()
        {
            try
            {
                _logger.LogInformation(_localizer["BackgroundJobs.GradeRecalculationStarted"]);

                var students = await _studentRepository.GetAllAsync();
                int updatedCount = 0;

                foreach (var student in students)
                {
                    student.RecalculateAverage();
                    await _studentRepository.UpdateAsync(student);
                    updatedCount++;
                }

                _logger.LogInformation(_localizer["BackgroundJobs.GradeRecalculationCompleted", updatedCount]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer["BackgroundJobs.GradeRecalculationFailed"]);
                throw;
            }
        }

        public async Task SendEnrollmentDeadlineNotificationsAsync()
        {
            try
            {
                _logger.LogInformation(_localizer["BackgroundJobs.EnrollmentNotificationsStarted"]);

                var courses = await _courseRepository.GetAllAsync();
                var today = DateTime.UtcNow.Date;
                var notificationsSent = 0;

                // Find courses with enrollment deadlines in the next 3 days
                var upcomingDeadlines = courses
                    .Where(c => (c.EnrollmentEndDate.Date - today).TotalDays <= 3 &&
                                (c.EnrollmentEndDate.Date - today).TotalDays >= 0)
                    .ToList();

                foreach (var course in upcomingDeadlines)
                {
                    var daysRemaining = (int)(course.EnrollmentEndDate.Date - today).TotalDays;
                    string message = daysRemaining == 0
                        ? _localizer["BackgroundJobs.EnrollmentDeadlineToday", course.Title]
                        : _localizer["BackgroundJobs.EnrollmentDeadlineApproaching", course.Title, daysRemaining];

                    //  send a notification to the teacher
                    await _notificationService.SendTeacherNotificationAsync(
                        course.TeacherId,
                        _localizer["BackgroundJobs.EnrollmentDeadlineNotificationSubject"],
                        message);

                    notificationsSent++;
                }

                _logger.LogInformation(_localizer["BackgroundJobs.EnrollmentNotificationsCompleted", notificationsSent]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer["BackgroundJobs.EnrollmentNotificationsFailed"]);
                throw;
            }
        }
    }
}
