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
    }
}