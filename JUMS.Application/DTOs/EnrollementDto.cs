using System.ComponentModel.DataAnnotations;

namespace JUMS.Application.DTOs
{
    public class EnrollmentDto
    {
        
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public decimal? Grade { get; set; }
        public bool CanApplyToFrance { get; set; }
        public CourseDto Course { get; set; }
        public StudentDto Student { get; set; }
    }
}