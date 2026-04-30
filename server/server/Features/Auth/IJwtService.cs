namespace server.Features.Auth;

public interface IJwtService
{
    string GenerateToken(string userId, string userName);
}
