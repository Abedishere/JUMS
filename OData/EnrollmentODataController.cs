using JUMS.Domain.Entities;
using JUMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("odata/[controller]")]
    public class EnrollmentODataController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentODataController(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            return Ok(enrollments.AsQueryable());
        }

        
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                return NotFound();
            return Ok(enrollment);
        }
    }
}