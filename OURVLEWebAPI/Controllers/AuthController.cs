using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OURVLEWebAPI.Entities;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OURVLEWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(OurvleContext context, IConfiguration config) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly OurvleContext _context = context;


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (login.UserId <= 0)
                return BadRequest("UserId is required.");

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == login.UserId);

            if (user == null)
            {
                return Unauthorized("Invalid userId");
            }

            if (login.Password != user.Password)
            {
                return Unauthorized("Invalid password");
            }

            var token = GenerateJwtToken(user.UserId, user.AccountType);
            return Ok(new { token });
        }

        private string GenerateJwtToken(int userId, string accountType)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, accountType)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );
            
            Console.WriteLine(token.ToString());

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
