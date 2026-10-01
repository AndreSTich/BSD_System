using BsdSystem.Api.DTOs;
using BSDSystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BsdSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BsdDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BsdDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _context.Employees
                .Include(e => e.Role) 
                .FirstOrDefaultAsync(e => e.Logine == request.Logine);

            if (user == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            if (user.Passworde != request.Password)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Logine),
                new Claim(ClaimTypes.Role, user.Role.Title) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new AuthResponseDto
            {
                Token = jwt,
                EmployeeId = user.Id,
                FullName = $"{user.LastName} {user.FirstName}",
                Role = user.Role.Title
            });
        }
    }
}