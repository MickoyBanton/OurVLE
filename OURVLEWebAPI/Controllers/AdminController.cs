using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
