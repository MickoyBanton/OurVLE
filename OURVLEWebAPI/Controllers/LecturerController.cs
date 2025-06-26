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
            var calenderEvent = await _context.Calendarevents.Where(ce => ce.CourseId == courseId).ToListAsync();

            if (calenderEvent.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(calenderEvent);

        }

        public async Task<ActionResult<Section>> GetCreatedSection(int courseId)
        {
            var section = await _context.Sections.Where(s => s.CourseId == courseId).ToListAsync();

            if (section.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(section);

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

        [HttpPost("section")]

        public async Task<ActionResult<Section>> AddSection(Section newSection)
        {

            if (newSection == null)
            {
                return NotFound();
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Sections.Add(newSection);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtAction(nameof(GetCreatedSection), new { id = newSection.CourseId }, newSection);

        }

        [HttpPost("section/item")]

        public async Task <ActionResult<Sectionitem>> AddSectionitem ([FromForm] Sectionitem newSectionItem, IFormFile file)
        {
            if (newSectionItem == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Sectionitems.Add(newSectionItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // unique file name
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                message = "Upload successful",
                item = newSectionItem
            });




        }
    }
}
