using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Shared;

namespace Server.Features.Auth.Login;

public class LoginHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginHandler(AppDbContext context, IPasswordHasher passwordHasher, ILogger<LoginHandler> logger, IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user == null)
        {
            _logger.LogWarning("Login failed: user not found. Email={Email}", request.Email);
            return Result<string>.Fail("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: wrong password. UserId={UserId}", user.Id);
            return Result<string>.Fail("Invalid email or password");
        }

        var token = _jwtService.GenerateToken(user.Id, user.UserName);
        _logger.LogInformation("User {UserId} logged in", user.Id);
        return Result<string>.Ok(token);
    }
}
