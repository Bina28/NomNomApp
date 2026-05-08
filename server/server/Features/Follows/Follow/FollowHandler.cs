using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Shared;
using Server.Features.Sse;

namespace Server.Features.Follows.Follow;

public class FollowHandler
{
    private readonly AppDbContext _context;
    private readonly SetConnectionManager _sseManager;
    private readonly ILogger<FollowHandler> _logger;

    public FollowHandler(AppDbContext context, SetConnectionManager sseManager, ILogger<FollowHandler> logger)
    {
        _context = context;
        _sseManager = sseManager;
        _logger = logger;
    }
    public async Task<Result<bool>> FollowUser(string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        if (currentUserId == targetUserId)
        {
            _logger.LogWarning("User {UserId} attempted to follow themselves", currentUserId);
            return Result<bool>.Fail("Cannot follow yourself");
        }

        var existingFollow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId, ct);

        if (existingFollow != null)
        {
            _logger.LogWarning("User {UserId} is already following user {TargetUserId}", currentUserId, targetUserId);
            return Result<bool>.Fail("Already following this user");
        }

        var currentUser = await _context.Users.FindAsync([currentUserId], ct);
        if (currentUser == null)
        {
            _logger.LogError("Authenticated user {UserId} not found in database", currentUserId);
            return Result<bool>.Fail("User not found");
        }

        var follow = new Follow
        {
            FollowerId = currentUserId,
            FollowingId = targetUserId
        };

        var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetUserId, ct);
        if (targetUser == null)
        {
            _logger.LogWarning("Target user {TargetUserId} not found for follow by user {UserId}", targetUserId, currentUserId);
            return Result<bool>.Fail("Target user not found");
        }

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync(ct);

        await _sseManager.BroadcastToAll("new_follow", new
        {
            followerId = currentUser.Id,
            followerName = currentUser.UserName,
            followingId = targetUser.Id,
            followingName = targetUser.UserName
        });

        _logger.LogInformation("User {UserId} followed user {TargetUserId}", currentUserId, targetUserId);
        return Result<bool>.Ok(true);
    }

}
