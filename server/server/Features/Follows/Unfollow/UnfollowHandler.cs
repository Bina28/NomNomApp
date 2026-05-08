using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Follows.Follow;
using Server.Features.Shared;

namespace Server.Features.Follows.Unfollow;

public class UnfollowHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<FollowHandler> _logger;

    public UnfollowHandler(AppDbContext context, ILogger<FollowHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> UnfollowUser(string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        var deleted = await _context.Follows
            .Where(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId)
            .ExecuteDeleteAsync(ct);

        if (deleted == 0)
        {
            _logger.LogWarning("User {UserId} attempted to unfollow user {TargetUserId} but was not following", currentUserId, targetUserId);
            return Result<bool>.Fail("Not following this user");
        }

        _logger.LogInformation("User {UserId} unfollowed user {TargetUserId}", currentUserId, targetUserId);
        return Result<bool>.Ok(true);
    }
}
