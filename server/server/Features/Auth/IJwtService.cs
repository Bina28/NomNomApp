namespace server.Features.Auth;

public interface IJwtService
{
    string GenereateToken(string userId, string userName);
}
