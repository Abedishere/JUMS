using JUMS.Domain.Entities;

namespace JUMS.Domain.Interfaces
{
    public interface ITeacherRepository
    {
        // Basic CRUD
        Task<Teacher> GetByIdAsync(Guid id);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(Teacher teacher);

        // Possibly get all Teachers 
        Task<List<Teacher>> GetAllAsync();

        
        Task<List<Teacher>> GetByCourseIdAsync(Guid courseId);
    }
}