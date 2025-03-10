using Microsoft.EntityFrameworkCore;
using JUMS.Domain.Entities;

namespace JUMS.EnrollmentService.Data
{
    public class EnrollmentContext : DbContext
    {
        public EnrollmentContext(DbContextOptions<EnrollmentContext> options)
            : base(options) { }

        // Reuse the same domain entities if that’s your design choice:
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EnrollmentContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}