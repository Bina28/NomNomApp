using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Domain;

namespace Server.Features.Auth;

public class RefreshTokenService
{
    private readonly AppDbContext _context;

    public RefreshTokenService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token && r.Revoked == null && r.Expires > DateTime.UtcNow);
    }

    public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
    {
        var refreshTokenRecord = new RefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(30)
        };
        _context.RefreshTokens.Add(refreshTokenRecord);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken token)
    {
        token.Revoked = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
