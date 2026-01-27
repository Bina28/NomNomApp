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

    public async Task<Result<bool>> FollowUser(string currentUserId, string targetUserId)
    {
        if (currentUserId == targetUserId)
        {
            return Result<bool>.Fail("Cannot follow yourself");
        }

        var existingFollow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId);

        if (existingFollow != null)
        {
            return Result<bool>.Fail("Already following this user");
        }

        var currentUser = await _context.Users.FindAsync(currentUserId);
        if (currentUser == null)
        {
            return Result<bool>.Fail("User not found");
        }

        var follow = new Follow
        {
            FollowerId = currentUserId,
            FollowingId = targetUserId
        };

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        await _sseManager.SendEventAsync(targetUserId, "new_follower", new
        {
            followerId = currentUser.Id,
            followerName = currentUser.UserName
        });

        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<FollowDto>>> GetFollowers(string userId)
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
            .ToListAsync();

        return Result<List<FollowDto>>.Ok(followers);
    }

    public async Task<Result<List<FollowDto>>> GetFollowing(string userId)
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
            .ToListAsync();

        return Result<List<FollowDto>>.Ok(following);
    }

    public async Task<Result<bool>> IsFollowing(string currentUserId, string targetUserId)
    {
        var isFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId);

        return Result<bool>.Ok(isFollowing);
    }

    public async Task<Result<bool>> UnfollowUser(string currentUserId, string targetUserId)
    {
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId);

        if (follow == null)
        {
            return Result<bool>.Fail("Not following this user");
        }

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();

        return Result<bool>.Ok(true);
    }
}
