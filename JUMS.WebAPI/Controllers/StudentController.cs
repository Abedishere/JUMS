using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using JUMS.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]

    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        // GET: api/Student
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentRepository.GetAllAsync();
            return Ok(students);
        }

        // GET: api/Student/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        // POST: api/Student
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentDto dto)
        {
            var student = new Student(Guid.NewGuid(), dto.Name);

            await _studentRepository.AddAsync(student);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        // PUT: api/Student/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StudentDto dto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null) return NotFound();

            existingStudent.Name = dto.Name;
            // If Student has other properties, map them here

            await _studentRepository.UpdateAsync(existingStudent);
            return NoContent();
        }

        // DELETE: api/Student/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return NotFound();

            await _studentRepository.DeleteAsync(student);
            return NoContent();
        }
    }
}
