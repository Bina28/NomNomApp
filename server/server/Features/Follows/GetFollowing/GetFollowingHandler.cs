using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Shared;

namespace Server.Features.Follows.GetFollowing;

public class GetFollowingHandler
{
    private readonly AppDbContext _context;
    public GetFollowingHandler(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<FollowingResponse>>> GetFollowing(string userId, CancellationToken ct = default)
    {
        var following = await _context.Follows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId)
            .Select(f => new FollowingResponse(f.Id, f.FollowingId, f.Following.UserName))
            .ToListAsync(ct);

        return Result<List<FollowingResponse>>.Ok((List<FollowingResponse>)following);
    }
}
