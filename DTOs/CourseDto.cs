using System.ComponentModel.DataAnnotations;

namespace JUMS.Application.DTOs;

public class CourseDto
{
    
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid TeacherId { get; set; }
    public TeacherDto Teacher { get; set; }
}