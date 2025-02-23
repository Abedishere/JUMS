using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations; // <-- Must be this one

namespace JUMS.Domain.Entities
{
    public class Student
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        
        public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

        
        public decimal? AverageGrade { get; private set; }
        public bool CanApplyToFrance { get; private set; }

        private Student() { }
        
        public Student(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        
        // Recalculate the students average grade 
        public void RecalculateAverage()
        {
            var validGrades = Enrollments
                .Where(e => e.Grade.HasValue)
                .Select(e => e.Grade.Value)
                .ToList();
                
            if (validGrades.Any())
            {
                AverageGrade = validGrades.Average();
                CanApplyToFrance = AverageGrade > 15;
            }
            else
            {
                AverageGrade = null;
                CanApplyToFrance = false;
            }
        }
    }
}