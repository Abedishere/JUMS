using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JUMS.Domain.Entities;
using JUMS.Application.DTOs;
using JUMS.Infrastructure.Persistence;

namespace JUMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController : ControllerBase
    {
        private readonly JUMSContext _context;
        
        public TimeSlotController(JUMSContext context)
        {
            _context = context;
        }
        
        // GET: api/TimeSlot
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var timeSlots = await _context.Set<TimeSlot>().ToListAsync();
            return Ok(timeSlots);
        }
        
        // GET: api/TimeSlot/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var timeSlot = await _context.Set<TimeSlot>().FindAsync(id);
            if (timeSlot == null)
                return NotFound();
            return Ok(timeSlot);
        }
        
        // POST: api/TimeSlot
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimeSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var timeSlot = new TimeSlot(Guid.NewGuid(), dto.StartTime, dto.EndTime, dto.TeacherId);
            await _context.Set<TimeSlot>().AddAsync(timeSlot);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = timeSlot.Id }, timeSlot);
        }
        
        // PUT: api/TimeSlot/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TimeSlotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var timeSlot = await _context.Set<TimeSlot>().FindAsync(id);
            if (timeSlot == null)
                return NotFound();
            
            timeSlot.StartTime = dto.StartTime;
            timeSlot.EndTime = dto.EndTime;
            timeSlot.TeacherId = dto.TeacherId;
            
            _context.Set<TimeSlot>().Update(timeSlot);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        // DELETE: api/TimeSlot/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var timeSlot = await _context.Set<TimeSlot>().FindAsync(id);
            if (timeSlot == null)
                return NotFound();
            
            _context.Set<TimeSlot>().Remove(timeSlot);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
