using BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(LibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _context.Database.EnsureCreated();
        }
        
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] UserCredentials credentials)
        {
            if (credentials == null || string.IsNullOrWhiteSpace(credentials.Name) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return BadRequest();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == credentials.Name);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"], 
                    audience: _configuration["Jwt:Audience"], 
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(45),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized("Invalid password");
        }

        
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);

        }

        
    }
}
