using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("odata/[controller]")]
    public class TeacherODataController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeacherODataController(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            return Ok(teachers.AsQueryable());
        }

        
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
                return NotFound();
            return Ok(teacher);
        }
    }
}