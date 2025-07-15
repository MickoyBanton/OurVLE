using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("[controller]")]
    [ApiController]
    public class AdminController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int CourseId)
        {

            var course = await _context.Courses.FindAsync(CourseId);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<Course>> AddCourse(Course newCourse)
        {
            if (newCourse == null)
            {
                return BadRequest("Invalid course data entered");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool courseExists = await _context.Courses
            .AnyAsync(c => c.CourseName.ToLower() == newCourse.CourseName.ToLower());

            if (courseExists)
            {
                return Conflict("A course with this name already exists.");
            }
            try
            {
                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtAction(nameof(GetCourse), new { id = newCourse.CourseId }, newCourse);

        }
    }
}
