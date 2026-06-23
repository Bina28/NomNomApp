using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Server.Features.Auth.GetAllUsers;
using Server.Features.Auth.GetCurrentUser;
using Server.Features.Auth.Login;
using Server.Features.Auth.Infrastructure.Jwt;
using Server.Features.Auth.RefreshTokens;
using Server.Features.Auth.Register;
using Server.Features.Auth.Shared;
using Server.Features.Shared;

namespace Server.Features.Auth;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly LoginHandler _login;
    private readonly RegisterHandler _register;
    private readonly GetCurrentUserHandler _getCurrentUser;
    private readonly GetAllUsersHandler _getAllUsers;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtService _jwtService;

    public AuthController(LoginHandler login, RegisterHandler register, GetCurrentUserHandler getCurrentUser, GetAllUsersHandler getAllUsers, IRefreshTokenService refreshToken, IJwtService jwtService)
    {
        _login = login;
        _register = register;
        _getCurrentUser = getCurrentUser;
        _getAllUsers = getAllUsers;
        _refreshTokenService = refreshToken;
        _jwtService = jwtService;
    }


    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _login.LoginAsync(request, ct);
        if (!result.Success || result.Data is null)
            return Problem(detail: result.Error, statusCode: 401);

        Response.Cookies.Append("access_token", result.Data.AccessToken, CreateAccessCookieOptions());
        Response.Cookies.Append("refresh_token", result.Data.RefreshToken, CreateRefreshCookieOptions());
        return Ok();
    }


    [EnableRateLimiting("auth")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _register.RegisterAsync(request, ct);
        if (!result.Success || result.Data is null)
            return Problem(detail: result.Error, statusCode: 400);

        Response.Cookies.Append("access_token", result.Data.AccessToken, CreateAccessCookieOptions());
        Response.Cookies.Append("refresh_token", result.Data.RefreshToken, CreateRefreshCookieOptions());
        return Ok();
    }

    [EnableRateLimiting("auth")]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken ct)
    {
        var token = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Missing refresh token.");

        var storedRefreshToken = await _refreshTokenService.GetRefreshTokenAsync(token, ct);
        if (storedRefreshToken is null)
            return Unauthorized("Invalid or expired refresh token.");

        var (accessToken, newRefreshToken) = _jwtService.GenerateToken(storedRefreshToken.UserId);
        await _refreshTokenService.RevokeRefreshTokenAsync(storedRefreshToken, newRefreshToken, ct);
        await _refreshTokenService.SaveRefreshTokenAsync(storedRefreshToken.UserId, newRefreshToken, ct);

        Response.Cookies.Append("access_token", accessToken, CreateAccessCookieOptions());
        Response.Cookies.Append("refresh_token", newRefreshToken, CreateRefreshCookieOptions());
        return Ok();
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken ct)
    {
        var result = await _getCurrentUser.GetCurrentUserAsync(User.GetUserId(), ct);

        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: 401);
    }


    [HttpGet("users")]
    [Authorize]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers([FromQuery] PageParameters pageParameters, CancellationToken ct)
    {
        var result = await _getAllUsers.GetUsersExceptCurrentAsync(User.GetUserId(), pageParameters, ct);

        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: 400);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(token))
        {
            var storedToken = await _refreshTokenService.GetRefreshTokenAsync(token);
            if (storedToken is not null)
                await _refreshTokenService.RevokeRefreshTokenAsync(storedToken);
        }
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return Ok();
    }

    private static CookieOptions CreateAccessCookieOptions() => new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
    };

    private static CookieOptions CreateRefreshCookieOptions() => new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddDays(30)
    };

}
