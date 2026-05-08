
using Server.Domain;
using Server.Features.Auth.Register;

namespace Server.Features.Auth;

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