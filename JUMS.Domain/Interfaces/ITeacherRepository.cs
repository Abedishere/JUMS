using JUMS.Domain.Entities;

namespace JUMS.Domain.Interfaces
{
    public interface ITeacherRepository
    {
        
        Task<Teacher> GetByIdAsync(Guid id);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(Teacher teacher);
        Task<List<Teacher>> GetAllAsync();

        
        Task<List<Teacher>> GetByCourseIdAsync(Guid courseId);
    }
}