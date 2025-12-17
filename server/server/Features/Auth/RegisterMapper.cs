
using server.Domain;
using server.Features.Auth.DTOs;

namespace server.Features.Auth;

public class RegisterMapper
{
    private readonly PasswordHasher _passwordHasher;

    public RegisterMapper(PasswordHasher hasher)
    {
        _passwordHasher = hasher;
    }
    public User ToEntity(RegisterRequest request)
    {
        return new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };
    }
}