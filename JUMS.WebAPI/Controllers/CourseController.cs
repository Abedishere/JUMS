using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JUMS.WebAPI.Controllers
{
    
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseRepository.GetAllAsync();
            return Ok(courses);
        }

        // GET: api/Course/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        // POST: api/Course
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseDto dto)
        {
            var course = new Course(
                Guid.NewGuid(),
                dto.Title,
                dto.StartDate,
                dto.EndDate,
                dto.TeacherId
            );

            await _courseRepository.AddAsync(course);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }

        // PUT: api/Course/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseDto dto)
        {
            var existingCourse = await _courseRepository.GetByIdAsync(id);
            if (existingCourse == null) return NotFound();

            existingCourse.Title = dto.Title;
            existingCourse.StartDate = dto.StartDate;
            existingCourse.EndDate = dto.EndDate;
            existingCourse.TeacherId = dto.TeacherId;

            await _courseRepository.UpdateAsync(existingCourse);
            return NoContent();
        }

        // DELETE: api/Course/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();

            await _courseRepository.DeleteAsync(course);
            return NoContent();
        }
    }
}
