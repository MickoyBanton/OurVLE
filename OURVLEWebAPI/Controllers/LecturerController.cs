﻿using Microsoft.AspNetCore.Authorization;
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

        public async Task<ActionResult<Calendarevent>> GetCalendarEvent(ulong courseId)
        {
            var calenderEvent = await _context.Calendarevents.Where(ce => ce.CourseId == courseId).ToListAsync();

            if (calenderEvent.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(calenderEvent);

        }

        public async Task<ActionResult<Section>> GetCreatedSection(int courseId)
        {
            var section = await _context.Sections.Where(s => s.CourseId == courseId).ToListAsync();

            if (section.Count == 0)
            {
                return NotFound("Calendar events not found");
            }

            return Ok(section);

        }

        public async Task<ActionResult<Sectionitem>> GetCreatedSectionItem(int SectionId)
        {
            var sectionItem = await _context.Sectionitems.Where(ce => ce.SectionId == SectionId).ToListAsync();

            if (sectionItem.Count == 0)
            {
                return NotFound("Section item not found");
            }

            return Ok(sectionItem);

        }


        public async Task<ActionResult<Assignment>> GetCreatedAssignment(int AssignmentId)
        {
            var assignment = await _context.Assignments.Where(a => a.AssignmentId == AssignmentId).ToListAsync();

            if (assignment.Count == 0)
            {
                return NotFound("Assignment not found");
            }

            return Ok(assignment);
        }

        public async Task<ActionResult<Grading>> GetCreatedGrade(int SubmissionId)
        {
            var grade = await _context.Gradings.Where(a => a.SubmissionId == SubmissionId).ToListAsync();

            if (grade.Count == 0)
            {
                return NotFound("Grade not found");
            }

            return Ok(grade);
        }


        [HttpGet("course")]
        public async Task<ActionResult<Course>> GetCourse()
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

            // Get the lecturer with courses included
            var lecturer = await _context.Lecturers.Include(s => s.Courses).FirstOrDefaultAsync(s => s.UserId == userId);


            if (lecturer == null)
            {
                return NotFound("Lecturer not found.");
            }

            var course = lecturer.Courses.Select(c => c.CourseName).ToList();

            // Return lecturer's Courses
            return Ok(course);
        }

        [HttpPost("calendar")]

        public async Task<ActionResult<Calendarevent>> AddCalenderEvent([FromBody] Calendarevent newCalenderEvent)

        {
            if (newCalenderEvent == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Calendarevents.Add(newCalenderEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetCalendarEvent), new { id = newCalenderEvent.EventId }, newCalenderEvent);

        }

        [HttpPost("section")]

        public async Task<ActionResult<Section>> AddSection(Section newSection)
        {

            if (newSection == null)
            {
                return NotFound();
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Sections.Add(newSection);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtAction(nameof(GetCreatedSection), new { id = newSection.CourseId }, newSection);

        }

        [HttpPost("section/item")]

        public async Task<ActionResult<Sectionitem>> AddSectionitem([FromForm] Sectionitem newSectionItem, IFormFile file)
        {

            if (newSectionItem == null)
            {
                return BadRequest("Invalid section item.");
            }
                

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Save metadata first to get the generated ItemId
            try
            {
                _context.Sectionitems.Add(newSectionItem);
                await _context.SaveChangesAsync(); // This sets newSectionItem.ItemId
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to save section item: " + ex.Message);
            }
                

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
                

            var fileName = $"{newSectionItem.ItemId}{Path.GetExtension(file.FileName)}";
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
                _context.Sectionitems.Remove(newSectionItem);
                await _context.SaveChangesAsync();

                return BadRequest("File upload failed. Item deleted from database: " + fileEx.Message);
            }

            return CreatedAtAction(nameof(GetCreatedSectionItem), new { sectionId = newSectionItem.SectionId }, newSectionItem);
        }

        [HttpPost ("assignment")]

        public async Task<ActionResult<Assignment>> AddAssignment(Assignment newAssignment)
        {
            if (newAssignment == null)
            {
                return BadRequest("Invalid assignment.");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Assignments.Add(newAssignment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetCreatedSectionItem), new { sectionId = newAssignment.AssignmentId }, newAssignment);

        }

        [HttpPost ("assignment/grade")]
        
        public async Task<ActionResult<Grading>> AddGrade(Grading newGrading)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                _context.Gradings.Add(newGrading);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtAction(nameof(GetCreatedGrade), new { submissionId = newGrading.SubmissionId }, newGrading);
        }
    }

}
