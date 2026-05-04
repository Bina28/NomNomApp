using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Shared;
using Server.Domain;
using Server.Features.Follows.DTOs;
using Server.Features.Sse;

namespace Server.Features.Follows;

public class FollowsHandler
{
    private readonly AppDbContext _context;
    private readonly SetConnectionManager _sseManager;
    private readonly ILogger<FollowsHandler> _logger;

    public FollowsHandler(AppDbContext context, SetConnectionManager sseManager, ILogger<FollowsHandler> logger)
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

    public async Task<Result<List<FollowerDto>>> GetFollowers(string userId, CancellationToken ct = default)
    {
        var followers = await _context.Follows
            .AsNoTracking()
            .Where(f => f.FollowingId == userId)
            .Select(f => new FollowerDto(f.Id, f.FollowerId, f.Follower.UserName))
            .ToListAsync(ct);

        return Result<List<FollowerDto>>.Ok(followers);
    }

    public async Task<Result<List<FollowingDto>>> GetFollowing(string userId, CancellationToken ct = default)
    {
        var following = await _context.Follows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId)
            .Select(f => new FollowingDto(f.Id, f.FollowingId, f.Following.UserName))
            .ToListAsync(ct);

        return Result<List<FollowingDto>>.Ok(following);
    }

    public async Task<Result<bool>> IsFollowing(string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        var isFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId, ct);

        return Result<bool>.Ok(isFollowing);
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
