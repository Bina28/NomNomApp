using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
using server.Features.Auth.Models;

namespace server.Features.Auth;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;

    public AuthController(JwtService service, AppDbContext context, PasswordHasher hasher)
    {
        _jwtService = service;
        _context = context;
        _passwordHasher = hasher;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized();
        }

        var token = _jwtService.GenereateToken(user.Id, user.UserName);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegsiterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
        {
            return BadRequest("User already exists");
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenereateToken(user.Id, user.UserName);
        return Ok(new { Token = token });

    }
}




