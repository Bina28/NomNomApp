using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Auth.Login;
using Server.Features.Shared;

namespace Server.Features.Auth.Register;

public class RegisterHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IJwtService _jwtService;
    private readonly RegisterMapper _registerMapper;

    public RegisterHandler(AppDbContext context, ILogger<LoginHandler> logger, IJwtService jwtService, RegisterMapper mapper)
    {
        _context = context;
        _logger = logger;
        _jwtService = jwtService;
        _registerMapper = mapper;
    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var emailTaken = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (emailTaken)
        {
            _logger.LogWarning("Registration failed: email already exists. Email={Email}", request.Email);
            return Result<string>.Fail("User already exists");
        }

        var newUser = _registerMapper.ToEntity(request);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(ct);

        var (accessToken, _) = _jwtService.GenerateToken(newUser.Id);
        _logger.LogInformation("Registered user {UserId} with name {UserName}", newUser.Id, newUser.UserName);
        return Result<string>.Ok(accessToken);
    }
}
