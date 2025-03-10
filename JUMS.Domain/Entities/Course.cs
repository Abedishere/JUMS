using System;
using System.ComponentModel.DataAnnotations;

namespace JUMS.Domain.Entities
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        public DateTime StartDate { get; set; }   // Course schedule start
        public DateTime EndDate { get; set; }     // Course schedule end
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        
        public int MaxStudents { get; set; }
        public DateTime EnrollmentStartDate { get; set; }
        public DateTime EnrollmentEndDate { get; set; }
        
        private Course() { }
        
        // Full constructor with explicit enrollment settings
        public Course(Guid id, string title, DateTime startDate, DateTime endDate, Guid teacherId,
            int maxStudents, DateTime enrollmentStartDate, DateTime enrollmentEndDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("Course start date must be before end date.");
            if (enrollmentStartDate >= startDate)
                throw new ArgumentException("Enrollment start date must be before the course start date.");
            if (enrollmentEndDate >= startDate)
                throw new ArgumentException("Enrollment end date must be before the course start date.");

            Id = id;
            Title = title;
            StartDate = startDate;
            EndDate = endDate;
            TeacherId = teacherId;
            MaxStudents = maxStudents;
            EnrollmentStartDate = enrollmentStartDate;
            EnrollmentEndDate = enrollmentEndDate;
        }
        
        // Defaults: MaxStudents=30, Enrollment starts 7 days before and ends 1 day before the course starts.
        public Course(Guid id, string title, DateTime startDate, DateTime endDate, Guid teacherId)
        {
            if (startDate >= endDate)
                throw new ArgumentException("Course start date must be before end date.");

            Id = id;
            Title = title;
            StartDate = startDate;
            EndDate = endDate;
            TeacherId = teacherId;
            MaxStudents = 30; // Default capacity
            EnrollmentStartDate = startDate.AddDays(-7); // Enrollment opens 7 days before the course starts
            EnrollmentEndDate = startDate.AddDays(-1);   // Enrollment closes 1 day before the course starts
        }

        // Determines if a given date falls within the enrollment window.
        public bool IsWithinEnrollmentWindow(DateTime currentDate)
        {
            return currentDate >= EnrollmentStartDate && currentDate <= EnrollmentEndDate;
        }
        
        // Checks if the course has available capacity based on current enrollment count.
        public bool HasCapacity(int currentEnrollmentCount)
        {
            return currentEnrollmentCount < MaxStudents;
        }
    }
}
