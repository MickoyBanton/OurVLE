using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Threading.Tasks;
using System.Security.Claims;


namespace OURVLEWebAPI.Controllers
{
    [Authorize(Roles = "student")]
    [ApiController]
    [Route("[controller]")]

    public class StudentController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;


        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out userId);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            var student = await _context.Students.FindAsync(userId);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> AddStudent(Student newStudent)
        {
            if (newStudent == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtAction(nameof(GetStudent), new { id = newStudent.UserId }, newStudent);

        }

        [HttpPost("course/{courseId}")]
        public async Task<ActionResult<Student>> RegisterCourse(ulong courseId)
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }
                
            // Get the student with courses included
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);


            if (student == null)
            {
                return NotFound("Student not found.");
            }

            if (student.Courses.Count == 6)
            {
                return BadRequest("Student is already doing 6 courses.");
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Check if student already has the course to avoid duplicates
            try
            {
                student.Courses.Add(course);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }

            // Return updated student info or some relevant response
            return CreatedAtAction(nameof(GetStudent), new { id = student.UserId }, new
            {
                student.UserId,
                student.FirstName,
                student.LastName,
                Courses = student.Courses.Select(c => c.CourseName).ToList()
            });
        }


        [HttpGet("course")]
        public async Task<ActionResult<Course>> GetCourse()
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            // Get the student with courses included
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);


            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var course = student.Courses.Select(c => c.CourseName).ToList();

            // Return student's Courses
            return Ok(course);
        }


        [HttpGet("calendar/{date}")]

        public async Task<ActionResult<Calendarevent>> GetCalendarEventByDate(DateTime date)
        {

            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            var events = await _context.Calendarevents
                .Where(e => e.DueDate.HasValue && e.DueDate.Value.Date == date.Date)
                .Where(e => e.Course!.Users.Any(s => s.UserId == userId))
                .Select(e => new
                {
                  e.EventId,
                  e.DueDate,
                  e.Title
                })
                .ToListAsync();

            if (events.Count == 0)
            {
                return NotFound("No calendar events found for this student on this date.");
            }

            return Ok(events);

        }

        [HttpPost("assignment")]

        public async Task<ActionResult<Assignment>> SubmitAssignment([FromForm] Submitassignment newAssignment, IFormFile file)
        {
            if (newAssignment == null)
            {
                return BadRequest("Invalid Submit Assignment");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            // Get the student with courses included
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);


            if (student == null)
            {
                return NotFound("Student not found.");
            }

            newAssignment.UserId = userId;

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Save metadata first to get the generated ItemId
            try
            {
                _context.Submitassignments.Add(newAssignment);
                await _context.SaveChangesAsync(); // This sets newSectionItem.ItemId
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to save section item: " + ex.Message);
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "SubmittedAssignment");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{newAssignment.SubmissionId}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }
            catch (Exception fileEx)
            {
                // Rollback the DB insert if file saving fails
                _context.Submitassignments.Remove(newAssignment);
                await _context.SaveChangesAsync();

                return BadRequest("File upload failed. Item deleted from database: " + fileEx.Message);
            }

            return Ok(new Submitassignment
            {
                SubmissionId = newAssignment.SubmissionId,
                AssignmentId = newAssignment.AssignmentId,
                UserId = newAssignment.UserId,
                SubmissionDate = newAssignment.SubmissionDate
            });
        }
    }
}