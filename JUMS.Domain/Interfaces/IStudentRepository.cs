using JUMS.Domain.Entities;

namespace JUMS.Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(Guid id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        Task<List<Student>> GetAllAsync();

      
    }
}