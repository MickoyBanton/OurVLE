using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OURVLEWebAPI.Entities;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OURVLEWebAPI.Controllers
{
    // Defines the route prefix as /Auth for this controller
    [Route("[controller]")]
    [ApiController]
    public class AuthController(OurvleContext context, IConfiguration config) : ControllerBase
    {
        // Dependency-injected configuration object for accessing appsettings.json (e.g., JWT key/issuer)
        private readonly IConfiguration _config = config;

        // Dependency-injected database context for accessing user account data
        private readonly OurvleContext _context = context;

        // Endpoint: POST /Auth/login
        // Authenticates a user and returns a JWT token if credentials are valid
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            // Validate the userId field (must be > 0)
            if (login.UserId <= 0)
                return BadRequest("UserId is required.");

            // Attempt to find the user in the database using the provided userId
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == login.UserId);

            // If no matching user is found, return Unauthorized (401)
            if (user == null)
            {
                return Unauthorized("Invalid userId");
            }

            // Check if the password matches
            if (login.Password != user.Password)
            {
                return Unauthorized("Invalid password");
            }

            // Generate a JWT token using the user's ID and role (account type)
            var token = GenerateJwtToken(user.UserId, user.AccountType);

            // Return the token to the client as a successful response
            return Ok(new { token });
        }

        // Generates a JSON Web Token (JWT) for authenticated users
        private string GenerateJwtToken(int userId, string accountType)
        {
            // Create a symmetric security key from the secret stored in configuration
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            // Create signing credentials using HMAC-SHA256 algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define claims to include in the token (User ID and Role)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, accountType)
            };

            // Build the JWT token with issuer, expiration time, claims, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: null, // Audience is optional or can be configured
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3), // Token valid for 3 hours
                signingCredentials: credentials
            );

            // Optional: Print token to console (for debugging)
            Console.WriteLine(token.ToString());

            // Return the encoded token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
