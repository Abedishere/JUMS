using System.ComponentModel.DataAnnotations;

namespace JUMS.Application.DTOs;

public class StudentDto
{
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<EnrollmentDto> Enrollments { get; set; } = new List<EnrollmentDto>();
    
    
}