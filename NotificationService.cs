using JUMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using JUMS.Infrastructure.Localization;
using System;
using System.Threading.Tasks;

namespace JUMS.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ISharedLocalizer _localizer;
        
        public NotificationService(
            ILogger<NotificationService> logger,
            ISharedLocalizer localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }
        
        public async Task SendStudentNotificationAsync(Guid studentId, string subject, string message)
        {
            
            _logger.LogInformation(_localizer["Notifications.SentToStudent", studentId, subject]);
            
            // Simulate async operation
            await Task.Delay(100);
        }
        
        public async Task SendTeacherNotificationAsync(Guid teacherId, string subject, string message)
        {
            
            _logger.LogInformation(_localizer["Notifications.SentToTeacher", teacherId, subject]);
            
            // Simulate async operation
            await Task.Delay(100);
        }
    }
}