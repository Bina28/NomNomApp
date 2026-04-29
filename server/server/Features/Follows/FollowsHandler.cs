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

    public FollowsHandler(AppDbContext context, SetConnectionManager sseManager)
    {
        _context = context;
        _sseManager = sseManager;
    }

    public async Task<Result<bool>> FollowUser(string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        if (currentUserId == targetUserId)
        {
            return Result<bool>.Fail("Cannot follow yourself");
        }

        var existingFollow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId, ct);

        if (existingFollow != null)
        {
            return Result<bool>.Fail("Already following this user");
        }

        var currentUser = await _context.Users.FindAsync([currentUserId], ct);
        if (currentUser == null)
        {
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

        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<FollowDto>>> GetFollowers(string userId, CancellationToken ct = default)
    {
        var followers = await _context.Follows
            .Include(f => f.Follower)
            .Where(f => f.FollowingId == userId)
            .Select(f => new FollowDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowingId = f.FollowingId,
                Follower = new UserInfo { UserName = f.Follower.UserName }
            })
            .ToListAsync(ct);

        return Result<List<FollowDto>>.Ok(followers);
    }

    public async Task<Result<List<FollowDto>>> GetFollowing(string userId, CancellationToken ct = default)
    {
        var following = await _context.Follows
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .Select(f => new FollowDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowingId = f.FollowingId,
                Following = new UserInfo { UserName = f.Following.UserName }
            })
            .ToListAsync(ct);

        return Result<List<FollowDto>>.Ok(following);
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
            return Result<bool>.Fail("Not following this user");
        }

        return Result<bool>.Ok(true);
    }
}
