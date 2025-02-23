using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JUMS.Domain.Entities
{
    public class Teacher
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>(); 
        
        // Teaches available time slots.
        public ICollection<TimeSlot> AvailableTimeSlots { get; set; } = new List<TimeSlot>();
        
        private Teacher() { }
        
        public Teacher(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public void AddCourse(Course course)
        {
            Courses.Add(course);
        }
        
        public void AddTimeSlot(TimeSlot slot)
        {
            AvailableTimeSlots.Add(slot);
        }
    }
}