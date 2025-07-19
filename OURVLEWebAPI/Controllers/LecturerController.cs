using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    // Restrict access to only users with the "lecturer" role
    [Authorize(Roles = "lecturer")]
    [Route("[controller]")]
    [ApiController]
    public class LecturerController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;

        /// <summary>
        /// Extracts the authenticated user's ID from the claims.
        /// </summary>
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out userId);
        }

        /// <summary>
        /// Saves an uploaded file to a specified folder with the given file name.
        /// </summary>
        private async Task<bool> SaveFileAsync(IFormFile file, string folder, string fileName)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all calendar events for a specific course.
        /// </summary>
        public async Task<ActionResult<Calendarevent>> GetCalendarEvent(ulong courseId)
        {
            var calenderEvent = await _context.Calendarevents.Where(ce => ce.CourseId == courseId).ToListAsync();

            if (calenderEvent.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(calenderEvent);
        }

        /// <summary>
        /// Retrieves all sections created for a specific course.
        /// </summary>
        public async Task<ActionResult<Section>> GetCreatedSection(int courseId)
        {
            var section = await _context.Sections.Where(s => s.CourseId == courseId).ToListAsync();

            if (section.Count == 0)
            {
                return NotFound("Section not found");
            }

            return Ok(section);
        }

        /// <summary>
        /// Retrieves all section items created under a specific section.
        /// </summary>
        public async Task<ActionResult<Sectionitem>> GetCreatedSectionItem(int SectionId)
        {
            var sectionItem = await _context.Sectionitems.Where(ce => ce.SectionId == SectionId).ToListAsync();

            if (sectionItem.Count == 0)
            {
                return NotFound("Section item not found");
            }

            return Ok(sectionItem);
        }

        /// <summary>
        /// Retrieves assignment(s) by assignment ID.
        /// </summary>
        public async Task<ActionResult<Assignment>> GetCreatedAssignment(int AssignmentId)
        {
            var assignment = await _context.Assignments.Where(a => a.AssignmentId == AssignmentId).ToListAsync();

            if (assignment.Count == 0)
            {
                return NotFound("Assignment not found");
            }

            return Ok(assignment);
        }

        /// <summary>
        /// Retrieves grading information by submission ID.
        /// </summary>
        public async Task<ActionResult<Grading>> GetCreatedGrade(int SubmissionId)
        {
            var grade = await _context.Gradings.Where(a => a.SubmissionId == SubmissionId).ToListAsync();

            if (grade.Count == 0)
            {
                return NotFound("Grade not found");
            }

            return Ok(grade);
        }

        /// <summary>
        /// Returns the list of courses assigned to the currently authenticated lecturer.
        /// </summary>
        [HttpGet("course")]
        public async Task<ActionResult<Course>> GetCourse()
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            var lecturer = await _context.Lecturers.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);

            if (lecturer == null)
            {
                return NotFound("Lecturer not found.");
            }

            var course = lecturer.Courses.Select(c => c.CourseName).ToList();
            return Ok(course);
        }

        /// <summary>
        /// Adds a new calendar event for a course.
        /// </summary>
        [HttpPost("calendar")]
        public async Task<ActionResult<Calendarevent>> AddCalenderEvent([FromBody] Calendarevent newCalenderEvent)
        {
            if (newCalenderEvent == null)
            {
                return BadRequest("Invalid calendar data.");
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

        /// <summary>
        /// Adds a new section to a course.
        /// </summary>
        [HttpPost("section")]
        public async Task<ActionResult<Section>> AddSection(Section newSection)
        {
            if (newSection == null)
            {
                return BadRequest("Invalid section data.");
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

        /// <summary>
        /// Adds a new section item (e.g., file upload or content) to a section.
        /// </summary>
        [HttpPost("section/item")]
        public async Task<ActionResult<Sectionitem>> AddSectionitem([FromForm] Sectionitem newSectionItem, IFormFile file)
        {
            if (newSectionItem == null)
            {
                return BadRequest("Invalid section item.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                _context.Sectionitems.Add(newSectionItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to save section item: " + ex.Message);
            }

            var fileName = $"{newSectionItem.ItemId}{Path.GetExtension(file.FileName)}";
            string folderName = "Uploads";

            var saved = await SaveFileAsync(file, folderName, fileName);

            if (!saved)
            {
                _context.Sectionitems.Remove(newSectionItem);
                await _context.SaveChangesAsync();
                return BadRequest("File upload failed. Section item was removed.");
            }

            return CreatedAtAction(nameof(GetCreatedSectionItem), new { sectionId = newSectionItem.SectionId }, newSectionItem);
        }

        /// <summary>
        /// Adds a new assignment. Ensures the assignment date is not in the past.
        /// </summary>
        [HttpPost("assignment")]
        public async Task<ActionResult<Assignment>> AddAssignment(Assignment newAssignment)
        {
            if (newAssignment == null)
            {
                return BadRequest("Invalid assignment.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newAssignment.Date < DateTime.Today)
            {
                return BadRequest("Assignment date cannot be in the past.");
            }

            try
            {
                _context.Assignments.Add(newAssignment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetCreatedSectionItem), new { sectionId = newAssignment.AssignmentId }, newAssignment);
        }

        /// <summary>
        /// Adds a new grade for a submitted assignment.
        /// </summary>
        [HttpPost("assignment/grade")]
        public async Task<ActionResult<Grading>> AddGrade(Grading newGrading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Gradings.Add(newGrading);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetCreatedGrade), new { submissionId = newGrading.SubmissionId }, newGrading);
        }
    }
}
