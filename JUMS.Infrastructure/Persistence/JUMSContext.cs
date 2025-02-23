using JUMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JUMS.Infrastructure.Persistence
{
    public class JUMSContext : DbContext
    {
        public JUMSContext(DbContextOptions<JUMSContext> options)
            : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // if One Teacher has many Courses
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId);

            
        }
    }
}