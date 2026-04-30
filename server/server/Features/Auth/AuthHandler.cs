using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
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
    private readonly ILogger<AuthHandler> _logger;

    public AuthHandler(IJwtService service, AppDbContext context, IPasswordHasher hasher, RegisterMapper registerMapper, ILogger<AuthHandler> logger)
    {
        _jwtService = service;
        _context = context;
        _passwordHasher = hasher;
        _registerMapper = registerMapper;
        _logger = logger;
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("LoginAsync started. Email={Email}", request.Email);
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user == null)
        {
            _logger.LogWarning("Login failed, user not found. Email={Email}", request.Email);
            return Result<string>.Fail("Invalid email or password");

        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed, wrong password. Email={Email}", request.Email);
            return Result<string>.Fail("Invalid email or password");
        }

        _logger.LogInformation("Login successful. Email={Email}", request.Email);
        var token = _jwtService.GenerateToken(user.Id, user.UserName);
        return Result<string>.Ok(token);

    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("RegisterAsync started. Email={Email}", request.Email);
        var user = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (user)
        {
            _logger.LogWarning("Registration failed, email already exists. Email={Email}", request.Email);
            return Result<string>.Fail("User already exists");
        }

        var newUser = _registerMapper.ToEntity(request);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Registration successful. Email={Email}", request.Email);
        var token = _jwtService.GenerateToken(newUser.Id, newUser.UserName);
        return Result<string>.Ok(token);
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(string userId, CancellationToken ct = default)
    {
        _logger.LogInformation("GetCurrentUserAsync started. UserId={UserId}", userId);

        var user = await _context.Users.FindAsync([userId], ct);

        if (user == null)
        {
            _logger.LogWarning("GetCurrentUserAsync failed, user not found. UserId={UserId}", userId);
            return Result<UserDto>.Fail("User not found");
        }

        _logger.LogInformation("GetCurrentUserAsync successful. UserId={UserId}", userId);
        return Result<UserDto>.Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        });
    }

    public async Task<Result<List<UserDto>>> GetUsersExceptCurrentAsync(string currentUserId, CancellationToken ct = default)
    {
        _logger.LogInformation("GetUsersExceptCurrentAsync started. CurrentUserId={CurrentUserId}", currentUserId);
        var users = await _context.Users
            .Where(u => u.Id != currentUserId)
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            })
            .ToListAsync(ct);

        _logger.LogInformation("GetUsersExceptCurrentAsync successful. Count={Count}", users.Count);
        return Result<List<UserDto>>.Ok(users);
    }
}
