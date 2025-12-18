using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Auth.DTOs;
using server.Features.Shared;

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

    public async Task<ResultValue<string>> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null  )
        {
            return Result.Fail<string>("User not found");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Fail<string>("Password is not valid");
        }

        var token = _jwtService.GenereateToken(user.Id, user.UserName);
        return Result.Success(token);

    }

    public async Task<ResultValue<string>> RegisterAsync(RegisterRequest request)
    {
        var user = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (user)
        {
            return Result.Fail<string>("User already exists");
        }

        var mapper = new RegisterMapper(_passwordHasher);
        var newUser = mapper.ToEntity(request);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenereateToken(newUser.Id, newUser.UserName);
        return Result.Success(token);
    }
}
