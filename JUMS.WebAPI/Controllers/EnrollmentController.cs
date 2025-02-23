using System;
using System.Threading.Tasks;
using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentController(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
        }

        // POST: api/Enrollment
        // Enroll a student in a course if enrollment window is open and capacity allows.
        [HttpPost]
        public async Task<IActionResult> Enroll([FromBody] EnrollDto enrollDto)
        {
            // Retrieve the course
            var course = await _courseRepository.GetByIdAsync(enrollDto.CourseId);
            if (course == null)
                return NotFound("Course not found.");

            // Check if enrollment is open
            if (!course.IsWithinEnrollmentWindow(DateTime.UtcNow))
                return BadRequest("Enrollment window is closed.");

            // Check current enrollment count
            var currentEnrollments = await _enrollmentRepository.GetByCourseIdAsync(course.Id);
            if (!course.HasCapacity(currentEnrollments.Count))
                return BadRequest("Course is full.");

            // Retrieve the student
            var student = await _studentRepository.GetByIdAsync(enrollDto.StudentId);
            if (student == null)
                return NotFound("Student not found.");

            // Create and add the enrollment
            var enrollment = new Enrollment(Guid.NewGuid(), student.Id, course.Id);
            await _enrollmentRepository.AddAsync(enrollment);

            // Update student's enrollments and recalculate average grade/eligibility
            student.Enrollments.Add(enrollment);
            student.RecalculateAverage();
            await _studentRepository.UpdateAsync(student);

            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
        }

        // GET: api/Enrollment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(Guid id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                return NotFound();
            return Ok(enrollment);
        }

        // PUT: api/Enrollment/{id}/grade
        // Update the grade for an enrollment and recalculate the student's average.
        [HttpPut("{id}/grade")]
        public async Task<IActionResult> UpdateGrade(Guid id, [FromBody] decimal newGrade)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                return NotFound("Enrollment not found.");

            enrollment.UpdateGrade(newGrade);
            await _enrollmentRepository.UpdateAsync(enrollment);

            var student = enrollment.Student;
            if (student == null)
                return BadRequest("Student information is missing.");

            student.RecalculateAverage();
            await _studentRepository.UpdateAsync(student);

            return NoContent();
        }
    }

    
    public class EnrollDto
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
    }
}
