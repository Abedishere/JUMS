using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("odata/[controller]")]
    public class StudentODataController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentODataController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

       
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var students = await _studentRepository.GetAllAsync();
            return Ok(students.AsQueryable());
        }

        
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound();
            return Ok(student);
        }
    }
}