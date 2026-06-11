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

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token && r.Revoked == null && r.Expires > DateTime.UtcNow, ct);
    }

    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, CancellationToken ct = default)
    {
        var refreshTokenRecord = new RefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(30)
        };
        _context.RefreshTokens.Add(refreshTokenRecord);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByToken = null, CancellationToken ct = default)
    {
        token.Revoked = DateTime.UtcNow;
        token.ReplacedByToken = replacedByToken;
        await _context.SaveChangesAsync(ct);
    }
}
