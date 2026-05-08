using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Auth.Shared;
using Server.Features.Shared;

namespace Server.Features.Auth.GetAllUsers;

public class GetAllUsersHandler
{
    private readonly AppDbContext _context;

    public GetAllUsersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserResponse>>> GetUsersExceptCurrentAsync(string currentUserId, CancellationToken ct = default)
    {
        var users = await _context.Users
            .Where(u => u.Id != currentUserId)
            .AsNoTracking()
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            })
            .ToListAsync(ct);

        return Result<List<UserResponse>>.Ok(users);
    }
}
