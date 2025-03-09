using JUMS.Domain.Entities;
using JUMS.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace JUMS.Infrastructure.Persistence
{
    public class JUMSContext : DbContext
    {
        private readonly string _tenant;
        
        public string Tenant => _tenant;

        public JUMSContext(DbContextOptions<JUMSContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            // Capture tenant from the provider
            _tenant = tenantProvider.GetTenant();
        }
        
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set the default schema dynamically based on the tenant.
            modelBuilder.HasDefaultSchema(_tenant);

            // Relationship: one Teacher has many Courses
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId);

            base.OnModelCreating(modelBuilder);
        }
    }

    // Custom model cache key factory to include tenant in the cache key.
    public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            if (context is JUMSContext tenantContext)
            {
                return (context.GetType(), tenantContext.Tenant);
            }
            return context.GetType();
        }
    }
}