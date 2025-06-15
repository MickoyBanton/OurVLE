using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    [Authorize(Roles = "lecturer, student, admin")]
    [Route("[controller]")]
    [ApiController]
    public class CoursesController(OurvleContext context) : ControllerBase
    {
        private readonly OurvleContext _context = context;

        [HttpGet]
        public async Task<ActionResult<Course>> GetCourse()
        {

            // Get the student with courses included
            var courses = await _context.Courses.ToListAsync();

            /*
            if (courses == null)
            {
                return NotFound("Courses not found.");
            }*/


            // Return Courses
            return Ok(courses);
        }
    }
}
