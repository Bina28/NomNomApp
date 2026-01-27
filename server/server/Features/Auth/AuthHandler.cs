using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Auth.DTOs;
using server.Features.Shared;
using Server.Features.Auth.DTOs;

namespace server.Features.Auth;

public class AuthHandler
{
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;

    public AuthHandler(JwtService service, AppDbContext context, PasswordHasher hasher)
    {
        _jwtService = service;
        _context = context;
        _passwordHasher = hasher;
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null  )
        {
            return Result<string>.Fail("User not Found");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<string>.Fail("User not Found");
        }

        var token = _jwtService.GenereateToken(user.Id, user.UserName);
        return Result<string>.Ok(token);

    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request)
    {
        var user = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (user)
        {
            return Result<string>.Fail("User already exists");
        }

        var mapper = new RegisterMapper(_passwordHasher);
        var newUser = mapper.ToEntity(request);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenereateToken(newUser.Id, newUser.UserName);
        return Result<string>.Ok(token);
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(string userId)
{
    if (string.IsNullOrEmpty(userId))
        return Result<UserDto>.Fail("Unauthorized");

    var user = await _context.Users.FindAsync(userId);

    if (user == null)
        return Result<UserDto>.Fail("User not found");

    return Result<UserDto>.Ok(new UserDto
    {
        Id = user.Id,
        Email = user.Email,
        UserName = user.UserName
    });
}

}
