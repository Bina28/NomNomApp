using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Shared;

namespace Server.Features.Follows.CheckFollowStatus;

public class CheckFollowStatusHandler
{
    private readonly AppDbContext _context;

    public CheckFollowStatusHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> IsFollowingAsync(string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        var isFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId, ct);

        return Result<bool>.Ok(isFollowing);
    }

}
