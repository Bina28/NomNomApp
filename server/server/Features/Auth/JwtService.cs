using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Features.Auth;

public class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;

    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public (string AccessToken, string RefreshToken) GenerateToken(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        return (accessToken, refreshToken);
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key!));
        var token = new JwtSecurityToken(
      issuer: _jwtOptions.Issuer,
      audience: _jwtOptions.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtOptions.ExpiryMinutes)),
      signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
      );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

}
