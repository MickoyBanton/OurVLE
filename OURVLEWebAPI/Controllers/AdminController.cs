using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    // Only users with the role "admin" can access endpoints in this controller
    [Authorize(Roles = "admin")]
    [Route("[controller]")]
    [ApiController]
    public class AdminController(OurvleContext context) : ControllerBase
    {
        // Injected database context to interact with the database
        private readonly OurvleContext _context = context;

        /// <summary>
        /// Retrieves course details based on the specified course ID.
        /// </summary>
        /// <param name="CourseId">The ID of the course to retrieve.</param>
        /// <returns>Course details if found; otherwise, NotFound response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int CourseId)
        {
            // Try to find the course with the given ID
            var course = await _context.Courses.FindAsync(CourseId);

            if (course == null)
            {
                // Return 404 if course not found
                return NotFound();
            }

            // Return course data if found
            return Ok(course);
        }

        /// <summary>
        /// Adds a new course to the system if it doesn't already exist.
        /// </summary>
        /// <param name="newCourse">The course information to be added.</param>
        /// <returns>The created course with 201 status or appropriate error response.</returns>
        [HttpPost]
        public async Task<ActionResult<Course>> AddCourse(Course newCourse)
        {
            // Check for null input
            if (newCourse == null)
            {
                return BadRequest("Invalid course data entered");
            }

            // Check model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a course with the same name (case-insensitive) already exists
            bool courseExists = await _context.Courses
                .AnyAsync(c => c.CourseName.ToLower() == newCourse.CourseName.ToLower());

            if (courseExists)
            {
                // Return 409 Conflict if course name already exists
                return Conflict("A course with this name already exists.");
            }

            try
            {
                // Add new course to the database
                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Return 400 Bad Request if an error occurs
                return BadRequest(ex.Message);
            }

            // Return 201 Created response with the newly created course
            return CreatedAtAction(nameof(GetCourse), new { id = newCourse.CourseId }, newCourse);
        }
    }
}
