using System.ComponentModel.DataAnnotations;

namespace JUMS.Application.DTOs;

public class TeacherDto
{
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<CourseDto> Courses { get; set; } = new List<CourseDto>();
    
}