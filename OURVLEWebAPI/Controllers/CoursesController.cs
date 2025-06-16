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

            
            if (courses.Count == 0)
            {
                return NotFound("Courses not found.");
            }


            // Return Courses
            return Ok(courses);
        }


        [HttpGet("member/{courseId}")]

        public async Task<ActionResult> GetMember(ulong courseId)
        {
            //Get course with student included

            var courseStudent = await _context.Courses.Include(c => c.Users).FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (courseStudent == null)
            {
                return NotFound("Course not found");
            }

            var courseLecturer = await _context.Courses.Include(c => c.UsersNavigation).FirstOrDefaultAsync(c => c.CourseId == courseId);

            if(courseLecturer == null)
            {
                return NotFound("Course not found");
            }


            var student = courseStudent.Users.Select(s => new { s.FirstName, s.LastName}).ToList();
            var lecturer = courseLecturer.UsersNavigation.Select(s => new { s.FirstName, s.LastName }).ToList();

            return Ok(new { student, lecturer });



        }
    }
}
