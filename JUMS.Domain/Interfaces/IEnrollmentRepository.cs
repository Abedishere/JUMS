using JUMS.Domain.Entities;

namespace JUMS.Domain.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> GetByIdAsync(Guid id);
        Task AddAsync(Enrollment enrollment);
        Task UpdateAsync(Enrollment enrollment);
        Task DeleteAsync(Enrollment enrollment);
        Task<List<Enrollment>> GetAllAsync();
        Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId);
        Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId);

        
        Task<List<Enrollment>> GetByTeacherIdAsync(Guid teacherId);
    }
}   