namespace Server.Features.Auth.Infrastructure.Jwt;

public interface IJwtService
{
    (string AccessToken, string RefreshToken) GenerateToken(string userId);
}
