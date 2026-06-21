using Server.Domain;

namespace Server.Features.Auth.RefreshTokens;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
    Task SaveRefreshTokenAsync(string userId, string refreshToken, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByToken = null, CancellationToken ct = default);
}
