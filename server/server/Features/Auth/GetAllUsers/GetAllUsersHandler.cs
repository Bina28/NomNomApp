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

    public async Task<Result<PageList<UserResponse>>> GetUsersExceptCurrentAsync(string currentUserId, PageParameters parameters, CancellationToken ct = default)
    {
        var query =  _context.Users
            .Where(u => u.Id != currentUserId)
            .AsNoTracking()
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            });
          

        var pagedUsers = await PageList<UserResponse>.CreateAsync(query, parameters.PageNumber, parameters.PageSize);

        return Result<PageList<UserResponse>>.Ok(pagedUsers);
    }
}
