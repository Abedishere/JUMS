using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JUMS.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly JUMSContext _context;
        private readonly IMemoryCache _cache;
        private const string CourseCacheKey = "Course_GetAll";

        public CourseRepository(JUMSContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task DeleteAsync(Course course)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync(CourseCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _context.Courses.Include(c => c.Teacher).ToListAsync();
            });
        }

        public async Task<List<Course>> GetByTeacherIdAsync(Guid teacherId)
        {
            // Not caching this query.
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

        private void InvalidateCache()
        {
            _cache.Remove(CourseCacheKey);
        }
    }
}
