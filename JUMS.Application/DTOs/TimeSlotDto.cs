using System.ComponentModel.DataAnnotations;

namespace JUMS.Application.DTOs
{
    public class TimeSlotDto
    {
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [Required]
        public Guid TeacherId { get; set; }
    }
}