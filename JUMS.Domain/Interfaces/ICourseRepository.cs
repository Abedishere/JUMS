using JUMS.Domain.Entities;

namespace JUMS.Domain.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(Guid id);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Course course);
        Task<List<Course>> GetAllAsync();

        
        Task<List<Course>> GetByTeacherIdAsync(Guid teacherId);
    }
}