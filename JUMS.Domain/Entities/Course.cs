using System;
using System.ComponentModel.DataAnnotations;

namespace JUMS.Domain.Entities
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        public DateTime StartDate { get; set; }   // (Course schedule start)
        public DateTime EndDate { get; set; }     // (Course schedule end)
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        
        
        public int MaxStudents { get; set; }
        public DateTime EnrollmentStartDate { get; set; }
        public DateTime EnrollmentEndDate { get; set; }
        
        
        private Course() { }
        
        
        public Course(Guid id, string title, DateTime startDate, DateTime endDate, Guid teacherId,
            int maxStudents, DateTime enrollmentStartDate, DateTime enrollmentEndDate)
        {
            Id = id;
            Title = title;
            StartDate = startDate;
            EndDate = endDate;
            TeacherId = teacherId;
            MaxStudents = maxStudents;
            EnrollmentStartDate = enrollmentStartDate;
            EnrollmentEndDate = enrollmentEndDate;
        }

        public Course(Guid newGuid, string dtoTitle, DateTime dtoStartDate, DateTime dtoEndDate, Guid dtoTeacherId)
        {
            throw new NotImplementedException();
        }

       
        public bool IsWithinEnrollmentWindow(DateTime currentDate)
        {
            return currentDate >= EnrollmentStartDate && currentDate <= EnrollmentEndDate;
        }
        
      
        public bool HasCapacity(int currentEnrollmentCount)
        {
            return currentEnrollmentCount < MaxStudents;
        }
    }
}