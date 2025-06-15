using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Threading.Tasks;
using System.Security.Claims;


namespace OURVLEWebAPI.Controllers
{
    [Authorize (Roles= "student")]
    [ApiController]
    [Route("[controller]")]

    public class StudentController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;
        

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var student = await _context.Students.FindAsync(userIdClaim);

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return NotFound("User not found.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID.");
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
        public async Task<ActionResult<Student>> GetCourse()
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



    }
}