using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Threading.Tasks;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    // Only users with the role 'student' can access this controller
    [Authorize(Roles = "student")]
    [ApiController]
    [Route("[controller]")]
    public class StudentController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;

        /// <summary>
        /// Attempts to extract the user's ID from their authentication claims.
        /// </summary>
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out userId);
        }

        /// <summary>
        /// Saves an uploaded file to a specified folder with the given filename.
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
        /// Retrieves student information based on the authenticated user ID.
        /// </summary>
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

        /// <summary>
        /// Adds a new student to the system.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Student>> AddStudent(Student newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest("Invalid student data.");
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

        /// <summary>
        /// Allows a student to register for a course if they haven't already exceeded the course limit.
        /// </summary>
        [HttpPost("course/{courseId}")]
        public async Task<ActionResult<Student>> RegisterCourse(ulong courseId)
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            // Load student and their enrolled courses
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

            try
            {
                student.Courses.Add(course);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            // Return student info and updated courses
            return CreatedAtAction(nameof(GetStudent), new { id = student.UserId }, new
            {
                student.UserId,
                student.FirstName,
                student.LastName,
                Courses = student.Courses.Select(c => c.CourseName).ToList()
            });
        }

        /// <summary>
        /// Retrieves all courses the authenticated student is enrolled in.
        /// </summary>
        [HttpGet("course")]
        public async Task<ActionResult<Course>> GetCourse()
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var course = student.Courses.Select(c => c.CourseName).ToList();
            return Ok(course);
        }

        /// <summary>
        /// Returns calendar events for the authenticated student on a specific date.
        /// </summary>
        [HttpGet("calendar/{date}")]
        public async Task<ActionResult<Calendarevent>> GetCalendarEventByDate(DateTime date)
        {
            if (!TryGetUserId(out int userId))
            {
                return BadRequest("Invalid user.");
            }

            var events = await _context.Calendarevents
                .Where(e => e.DueDate.HasValue && e.DueDate.Value.Date == date.Date)
                .Where(e => e.Course!.Users.Any(s => s.UserId == userId)) // Filter to only student’s courses
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

        /// <summary>
        /// Allows a student to submit an assignment along with a file upload.
        /// </summary>
        [HttpPost("assignments/submit")]
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

            // Save submission metadata before saving file
            try
            {
                _context.Submitassignments.Add(newAssignment);
                await _context.SaveChangesAsync(); // Sets SubmissionId
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to save assignment metadata: " + ex.Message);
            }

            // Prepare filename and path for saving file
            var fileName = $"{newAssignment.SubmissionId}{Path.GetExtension(file.FileName)}";
            string folderName = "SubmittedAssignment";

            var saved = await SaveFileAsync(file, folderName, fileName);

            if (!saved)
            {
                // Roll back metadata if file save failed
                _context.Submitassignments.Remove(newAssignment);
                await _context.SaveChangesAsync();
                return BadRequest("File upload failed.");
            }

            // Return confirmation response
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
