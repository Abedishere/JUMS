using System;
using System.Threading.Tasks;

namespace JUMS.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendStudentNotificationAsync(Guid studentId, string subject, string message);
        Task SendTeacherNotificationAsync(Guid teacherId, string subject, string message);
    }
}