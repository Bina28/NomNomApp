using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Features.Auth.DTOs;

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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _service.LoginAsync(request);
        if (!result.Success)
            return Unauthorized(new { result.Error });

        Response.Cookies.Append(
            "access_token",
            result.Data,
            CreateCookieOptions()
        );

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _service.RegisterAsync(request);
        if (!result.Success)
            return BadRequest(new { result.Error });

        Response.Cookies.Append(
            "access_token",
            result.Data,
            CreateCookieOptions()
        );

        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _service.GetCurrentUserAsync(userId);

        return result.Success
            ? Ok(result.Data)
            : Unauthorized();
    }


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
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };
    }
}




