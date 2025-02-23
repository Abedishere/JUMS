using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    // The route now includes a version placeholder.
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeacherController(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        // GET: api/v1/Teacher
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            return Ok(teachers);
        }

        // GET: api/v1/Teacher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return NotFound();
            return Ok(teacher);
        }

        // POST: api/v1/Teacher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeacherDto dto)
        {
            var teacher = new Teacher(Guid.NewGuid(), dto.Name);
            await _teacherRepository.AddAsync(teacher);
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id, version = "1.0" }, teacher);
        }

        // PUT: api/v1/Teacher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TeacherDto dto)
        {
            var existingTeacher = await _teacherRepository.GetByIdAsync(id);
            if (existingTeacher == null) return NotFound();

            existingTeacher.Name = dto.Name;
            await _teacherRepository.UpdateAsync(existingTeacher);
            return NoContent();
        }

        // DELETE: api/v1/Teacher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return NotFound();

            await _teacherRepository.DeleteAsync(teacher);
            return NoContent();
        }
    }
}
