using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OURVLEWebAPI.Entities;
using System.Linq;
using System.Security.Claims;

namespace OURVLEWebAPI.Controllers
{
    [Authorize(Roles = "lecturer, student, admin")]
    [Route("[controller]")]
    [ApiController]
    public class CoursesController(OurvleContext context) : ControllerBase
    {
        // Injecting the database context for querying the database
        private readonly OurvleContext _context = context;


        // Retrieves all courses in the system
        [HttpGet]
        public async Task<ActionResult<Course>> GetCourse()
        {
            // Retrieve all courses from the database
            var courses = await _context.Courses.ToListAsync();

            // If no courses are found, return 404
            if (courses.Count == 0)
            {
                return NotFound("Courses not found.");
            }

            // Return the list of courses with 200 OK
            return Ok(courses);
        }

        // Endpoint: GET /Courses/{courseId}/member
        // Retrieves all students and lecturers associated with a given course
        [HttpGet("{courseId}/member")]
        public async Task<ActionResult> GetMember(ulong courseId)
        {
            // Get course including its associated students (Users)
            var courseStudent = await _context.Courses
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (courseStudent == null)
            {
                return NotFound("Course not found");
            }

            // Get course including its associated lecturers (UsersNavigation)
            var courseLecturer = await _context.Courses
                .Include(c => c.UsersNavigation)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (courseLecturer == null)
            {
                return NotFound("Course not found");
            }

            // Select and project student names from Users
            var student = courseStudent.Users
                .Select(s => new { s.FirstName, s.LastName })
                .ToList();

            // Select and project lecturer names from UsersNavigation
            var lecturer = courseLecturer.UsersNavigation
                .Select(s => new { s.FirstName, s.LastName })
                .ToList();

            // Return both student and lecturer lists
            return Ok(new { student, lecturer });
        }



        // Endpoint: GET /Courses/{courseId}/sectionitems
        // Retrieves all section items belonging to a specific course
        [HttpGet("{courseId}/sectionitems")]
        public async Task<ActionResult<IEnumerable<object>>> GetSectionItemsByCourse(int courseId)
        {
            // Step 1: Get all sections for the specified course
            var sections = await _context.Sections
                .Where(s => s.CourseId == courseId)
                .Select(s => new
                {
                    s.SectionId,
                    s.SectionName
                })
                .ToListAsync();

            if (sections == null || !sections.Any())
                return NotFound("No sections found for this course.");

            // Step 2: Get all section items related to those sections
            var sectionIds = sections.Select(s => s.SectionId).ToList();

            var sectionItems = await _context.Sectionitems
                .Where(si => sectionIds.Contains(si.SectionId.Value))
                .ToListAsync();

            if (sectionItems == null || !sectionItems.Any())
                return NotFound("No section items found for this course.");

            // Step 3: Perform the join in memory (EF-compatible)
            var result = sectionItems
                .Join(
                    sections,
                    si => si.SectionId,
                    s => s.SectionId,
                    (si, s) => new
                    {
                        si.ItemId,
                        si.SectionId,
                        si.SectionItem,
                        si.FileType,
                        SectionName = s.SectionName
                    }
                )
                .ToList();

            return Ok(result);
        }




        [HttpGet("{courseId}/assignment")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetCourseAssignment(ulong courseId)
        {
            var assignment = await _context.Assignments.Where(s => s.CourseId == courseId).ToListAsync();

            if (assignment == null)
            {
                return NotFound("No assignment found for this course.");
            }

            // Return the list of assignment
            return Ok(assignment);
        }

    }
}
