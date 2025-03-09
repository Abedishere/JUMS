using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JUMS.Infrastructure.Persistence.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly JUMSContext _context;
        private readonly IMemoryCache _cache;
        private const string EnrollmentCacheKey = "Enrollment_GetAll";

        public EnrollmentRepository(JUMSContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Enrollment> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                    .ThenInclude(s => s.Enrollments)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task DeleteAsync(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync(EnrollmentCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .ToListAsync();
            });
        }

        public async Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetByTeacherIdAsync(Guid teacherId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Course.TeacherId == teacherId)
                .ToListAsync();
        }

        private void InvalidateCache()
        {
            _cache.Remove(EnrollmentCacheKey);
        }
    }
}
