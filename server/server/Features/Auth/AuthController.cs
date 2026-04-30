using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Features.Auth.DTOs;
using Server.Features.Auth.DTOs;

namespace server.Features.Auth;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthHandler _service;

    public AuthController(AuthHandler service)
    {
        _service = service;
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _service.LoginAsync(request, ct);
        if (!result.Success || string.IsNullOrEmpty(result.Data))
            return Problem(detail: result.Error, statusCode: 401);

        Response.Cookies.Append(
            "access_token",
            result.Data,
            CreateCookieOptions()
        );

        return Ok();
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _service.RegisterAsync(request, ct);
        if (!result.Success || string.IsNullOrEmpty(result.Data))
            return Problem(detail: result.Error, statusCode: 400);

        Response.Cookies.Append(
            "access_token",
            result.Data,
            CreateCookieOptions()
        );

        return Ok();
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me(CancellationToken ct)
    {
        var result = await _service.GetCurrentUserAsync(User.GetUserId(), ct);
        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: 401);
    }


    [HttpGet("users")]
    [Authorize]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers(CancellationToken ct)
    {
        var result = await _service.GetUsersExceptCurrentAsync(User.GetUserId(), ct);
        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: 400);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return Ok();
    }

    private static CookieOptions CreateCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };
    }

}




