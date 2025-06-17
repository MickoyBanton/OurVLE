using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    [Authorize(Roles = "lecturer")]
    [Route("[controller]")]
    [ApiController]
    public class LecturerController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;

        public async Task<ActionResult<Calendarevent>> GetCalendarEvent(ulong courseId)
        {
            var calenderEvent = await _context.Calendarevents.Where(ce => ce.CourseId == courseId).ToListAsync(); ;

            if (calenderEvent.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(calenderEvent);

        }

        [HttpGet("course")]
        public async Task<ActionResult<Course>> GetCourse()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return NotFound("User not found.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID.");
            }

            // Get the lecturer with courses included
            var lecturer = await _context.Lecturers.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);


            if (lecturer == null)
            {
                return NotFound("Lecturer not found.");
            }

            var course = lecturer.Courses.Select(c => c.CourseName).ToList();

            // Return lecturer's Courses
            return Ok(course);
        }

        [HttpPost("calendar")]

        public async Task<ActionResult<Calendarevent>> CreateCalenderEvent([FromBody] Calendarevent newCalenderEvent)

        {
            if (newCalenderEvent == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Calendarevents.Add(newCalenderEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetCalendarEvent), new { id = newCalenderEvent.EventId }, newCalenderEvent);

        }
    }
}
