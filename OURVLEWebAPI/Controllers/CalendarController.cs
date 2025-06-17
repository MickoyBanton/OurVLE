using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;

namespace OURVLEWebAPI.Controllers
{
    [Authorize(Roles = "lecturer, student, admin")]
    [Route("[controller]")]
    [ApiController]
    public class CalendarController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;

        [HttpGet("{courseId}")]
        public async Task<ActionResult<Calendarevent>> GetCalendarEvent(ulong courseId)
        {
            var calenderEvent = await _context.Calendarevents.Where(ce => ce.CourseId == courseId).ToListAsync(); ;

            if (calenderEvent.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(calenderEvent);

        }
        /*
        [HttpGet("date/{date}")]

        public async Task<ActionResult<Calendarevent>> GetCalendarEventByDate(DateTime date)
        {
            var calenderEvent = await _context.Calendarevents.Where(ce => ce.CourseId == courseId).ToListAsync(); ;
        } */
    }
}
