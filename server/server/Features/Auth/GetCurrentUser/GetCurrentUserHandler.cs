using Server.Data;
using Server.Features.Auth.Shared;
using Server.Features.Shared;

namespace Server.Features.Auth.GetCurrentUser;

public class GetCurrentUserHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetCurrentUserHandler> _logger;

    public GetCurrentUserHandler(AppDbContext context, ILogger<GetCurrentUserHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> GetCurrentUserAsync(string userId, CancellationToken ct = default)
    {
        var user = await _context.Users.FindAsync([userId], ct);

        if (user == null)
        {
            _logger.LogError("Authenticated user {UserId} not found in database", userId);
            return Result<UserResponse>.Fail("User not found");
        }

        return Result<UserResponse>.Ok(new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        });
    }
}
