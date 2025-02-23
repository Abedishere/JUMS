using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JUMS.Infrastructure.Persistence.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly JUMSContext _context;
        private readonly IMemoryCache _cache;
        private const string TeacherCacheKey = "Teacher_GetAll";

        public TeacherRepository(JUMSContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Teacher> GetByIdAsync(Guid id)
        {
            return await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task DeleteAsync(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task<List<Teacher>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync(TeacherCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _context.Teachers.Include(t => t.Courses).ToListAsync();
            });
        }

        public async Task<List<Teacher>> GetByCourseIdAsync(Guid courseId)
        {
            // For simplicity, not caching this query.
            return await _context.Teachers
                .Include(t => t.Courses)
                .Where(t => t.Courses.Any(c => c.Id == courseId))
                .ToListAsync();
        }

        private void InvalidateCache()
        {
            _cache.Remove(TeacherCacheKey);
        }
    }
}
