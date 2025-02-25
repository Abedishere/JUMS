using System.Threading.Tasks;

namespace JUMS.Domain.Interfaces
{
    public interface IBackgroundJobService
    {
        void RegisterRecurringJobs();
        Task RecalculateStudentGradesAsync();
        Task SendEnrollmentDeadlineNotificationsAsync();
    }
}