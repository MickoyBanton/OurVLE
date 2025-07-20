using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;

namespace OURVLEWebAPI.Controllers
{
    // Restricts access to users with lecturer, student, or admin roles
    [Authorize(Roles = "lecturer, student, admin")]

    // Route will be matched as /Calendar
    [Route("[controller]")]

    // Marks this class as an API controller (enables model binding, validation, etc.)
    [ApiController]
    public class CalendarController(OurvleContext context) : ControllerBase
    {
        // Injects the database context for querying the database
        private readonly OurvleContext _context = context;

        // HTTP GET endpoint: /Calendar/{courseId}
        // Retrieves all calendar events for a specific course
        [HttpGet("{courseId}")]
        public async Task<ActionResult<Calendarevent>> GetCalendarEvent(ulong courseId)
        {
            // Fetch all calendar events associated with the given courseId
            var calenderEvent = await _context.Calendarevents
                                              .Where(ce => ce.CourseId == courseId)
                                              .ToListAsync();

            // If no events found, return 404 Not Found
            if (calenderEvent.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            // Return the list of calendar events with 200 OK
            return Ok(calenderEvent);
        }
    }
}
