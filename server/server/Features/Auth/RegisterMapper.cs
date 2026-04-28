
using server.Domain;
using server.Features.Auth.DTOs;

namespace server.Features.Auth;

public class RegisterMapper
{
    private readonly IPasswordHasher _passwordHasher;

    public RegisterMapper(IPasswordHasher hasher)
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