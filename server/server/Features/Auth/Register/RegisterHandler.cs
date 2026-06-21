using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Auth.Infrastructure.Jwt;
using Server.Features.Auth.Login;
using Server.Features.Auth.RefreshTokens;
using Server.Features.Shared;

namespace Server.Features.Auth.Register;

public class RegisterHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IJwtService _jwtService;
    private readonly RegisterMapper _registerMapper;
    private readonly IRefreshTokenService _refreshTokenService;

    public RegisterHandler(AppDbContext context, ILogger<LoginHandler> logger, IJwtService jwtService, RegisterMapper mapper, IRefreshTokenService refreshTokenService)
    {
        _context = context;
        _logger = logger;
        _jwtService = jwtService;
        _registerMapper = mapper;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var emailTaken = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (emailTaken)
        {
            _logger.LogWarning("Registration failed: email already exists. Email={Email}", request.Email);
            return Result<LoginResponse>.Fail("User already exists");
        }

        var newUser = _registerMapper.ToEntity(request);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(ct);

        var (accessToken, refreshToken) = _jwtService.GenerateToken(newUser.Id);
        await _refreshTokenService.SaveRefreshTokenAsync(newUser.Id, refreshToken, ct);
        _logger.LogInformation("Registered user {UserId} with name {UserName}", newUser.Id, newUser.UserName);
        return Result<LoginResponse>.Ok(new LoginResponse(accessToken, refreshToken));
    }
}
