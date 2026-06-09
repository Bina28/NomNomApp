namespace Server.Features.Auth;

public interface IJwtService
{
    (string AccessToken, string RefreshToken) GenerateToken(string userId);
}
