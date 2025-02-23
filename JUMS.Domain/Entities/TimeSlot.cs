using System;
using System.ComponentModel.DataAnnotations;

namespace JUMS.Domain.Entities
{
    public class TimeSlot
    {
        [Key]
        public Guid Id { get; set; }
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        // Optional: Associate a time slot with a teacher.
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        
        private TimeSlot() { }
        
        public TimeSlot(Guid id, DateTime startTime, DateTime endTime, Guid teacherId)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            TeacherId = teacherId;
        }
    }
}