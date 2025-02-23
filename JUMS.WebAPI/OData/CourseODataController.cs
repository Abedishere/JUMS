using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("odata/[controller]")]
    public class CourseODataController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CourseODataController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var courses = await _courseRepository.GetAllAsync();
            return Ok(courses.AsQueryable());
        }

        
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();
            return Ok(course);
        }
    }
}