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

        [HttpGet("course")]
        public async Task<ActionResult<Lecturer>> GetCourse()
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
            var lecturer = await _context.Lecturers.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);


            if (lecturer == null)
            {
                return NotFound("Lecturer not found.");
            }

            var course = lecturer.Courses.Select(c => c.CourseName).ToList();

            // Return student's Courses
            return Ok(course);
        }
    }
}
