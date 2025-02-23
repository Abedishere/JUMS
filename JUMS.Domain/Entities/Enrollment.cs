using System.ComponentModel.DataAnnotations;

namespace JUMS.Domain.Entities
{
    public class Enrollment
    {
        [Key]
        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public Student Student { get; private set; }
        public Guid CourseId { get; private set; }
        public Course Course { get; private set; }
        
        public decimal? Grade { get; private set; }
        
        
        public bool CanApplyToFrance { get; private set; }

        private Enrollment() { }

        public Enrollment(Guid id, Guid studentId, Guid courseId)
        {
            Id = id;
            StudentId = studentId;
            CourseId = courseId;
        }

        // Update the grade for this enrollment.
        public void UpdateGrade(decimal grade)
        {
            Grade = grade;
            
        }
        
    }
}