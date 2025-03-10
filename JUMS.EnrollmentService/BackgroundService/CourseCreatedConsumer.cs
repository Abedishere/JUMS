using MassTransit;
using Microsoft.Extensions.Logging;
using JUMS.EnrollmentService.Data;
using JUMS.Domain.Entities;
using System.Threading.Tasks;

namespace JUMS.EnrollmentService.Messaging
{
    public class CourseCreatedConsumer : IConsumer<CourseCreatedMessage>
    {
        private readonly EnrollmentContext _context;
        private readonly ILogger<CourseCreatedConsumer> _logger;

        public CourseCreatedConsumer(EnrollmentContext context, ILogger<CourseCreatedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CourseCreatedMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received course created message: {Message}", message);

            // Check if course already exists
            var existingCourse = await _context.Courses.FindAsync(message.Id);
            if (existingCourse != null)
            {
                _logger.LogInformation("Course {Id} already exists. Skipping.", message.Id);
                return;
            }

            // Create a new Course entity from the received message
            var newCourse = new Course(
                message.Id,
                message.Title,
                message.StartDate,
                message.EndDate,
                message.TeacherId);

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New course created: {Id}", newCourse.Id);
        }
    }

    public class CourseCreatedMessage
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid TeacherId { get; set; }
    }
}