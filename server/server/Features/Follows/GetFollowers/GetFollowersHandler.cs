using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Shared;

namespace Server.Features.Follows.GetFollowers;

public class GetFollowersHandler
{
    private readonly AppDbContext _context;

    public GetFollowersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<FollowerResponse>>> GetFollowersAsync(string userId, CancellationToken ct = default)
    {
        var followers = await _context.Follows
            .AsNoTracking()
            .Include(c => c.Follower)
            .Where(f => f.FollowingId == userId)
            .Select(f => new FollowerResponse(f.Id, f.FollowerId, f.Follower.UserName))
            .ToListAsync(ct);

        return Result<List<FollowerResponse>>.Ok(followers);
    }
}
