using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Auth.DTOs;
using server.Features.Shared;
using Server.Features.Auth.DTOs;

namespace server.Features.Auth;

public class AuthHandler
{
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly RegisterMapper _registerMapper;

    public AuthHandler(IJwtService service, AppDbContext context, IPasswordHasher hasher, RegisterMapper registerMapper)
    {
        _jwtService = service;
        _context = context;
        _passwordHasher = hasher;
        _registerMapper = registerMapper;
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user == null)
        {
            return Result<string>.Fail("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<string>.Fail("Invalid email or password");
        }

        var token = _jwtService.GenereateToken(user.Id, user.UserName);
        return Result<string>.Ok(token);

    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var user = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (user)
        {
            return Result<string>.Fail("User already exists");
        }

        var newUser = _registerMapper.ToEntity(request);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(ct);

        var token = _jwtService.GenereateToken(newUser.Id, newUser.UserName);
        return Result<string>.Ok(token);
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(string userId, CancellationToken ct = default)
{
    if (string.IsNullOrEmpty(userId))
        return Result<UserDto>.Fail("Unauthorized");

    var user = await _context.Users.FindAsync([userId], ct);

    if (user == null)
        return Result<UserDto>.Fail("User not found");

    return Result<UserDto>.Ok(new UserDto
    {
        Id = user.Id,
        Email = user.Email,
        UserName = user.UserName
    });
}

    public async Task<Result<List<UserDto>>> GetAllUsersAsync(string? currentUserId, CancellationToken ct = default)
    {
        var users = await _context.Users
            .Where(u => u.Id != currentUserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            })
            .ToListAsync(ct);

        return Result<List<UserDto>>.Ok(users);
    }
}
