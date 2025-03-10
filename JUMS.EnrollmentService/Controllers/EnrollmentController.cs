using Microsoft.AspNetCore.Mvc;
using JUMS.EnrollmentService.Data;
using Microsoft.EntityFrameworkCore;

namespace JUMS.EnrollmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentContext _context;

        public EnrollmentController(EnrollmentContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

            return Ok(enrollments);
        }
        
        // to do;
        // Additional endpoints for creating enrollments, updating, deleting, 
        
    }
}