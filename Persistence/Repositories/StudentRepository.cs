using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JUMS.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly JUMSContext _context;
        private readonly IMemoryCache _cache;
        private const string StudentCacheKey = "Student_GetAll";

        public StudentRepository(JUMSContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Student> GetByIdAsync(Guid id)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync(StudentCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _context.Students.Include(s => s.Enrollments).ToListAsync();
            });
        }

        private void InvalidateCache()
        {
            _cache.Remove(StudentCacheKey);
        }
    }
}