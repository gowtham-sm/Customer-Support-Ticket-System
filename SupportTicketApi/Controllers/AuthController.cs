using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupportTicketApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SupportTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(SupportTicketDbContext context, IConfiguration config) : ControllerBase
{
    // DTO for the incoming JSON payload
    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Validate User
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

        if (user == null)
            return Unauthorized(new { Message = "Invalid username or password" });

        // 2. Generate JWT Claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role) // Injects 'User' or 'Admin'
        };

        // 3. Mint the Token
        var jwtSettings = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        // 4. Return token and role to the desktop client
        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = user.Role
        });
    }
}