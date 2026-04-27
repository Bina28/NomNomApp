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


    // IActionResult: returnerer ingen data, bare setter cookie
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _service.LoginAsync(request);
        if (!result.Success)
            return Problem(detail: result.Error, statusCode: 401);

        Response.Cookies.Append(
            "access_token",
            result.Data,
            CreateCookieOptions()
        );

        return Ok();
    }

    // IActionResult: returnerer ingen data, bare setter cookie
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _service.RegisterAsync(request);
        if (!result.Success)
           return Problem(detail: result.Error, statusCode: 400);

        Response.Cookies.Append(
            "access_token",
            result.Data,
            CreateCookieOptions()
        );

        return Ok();
    }

    // TODO: bør være ActionResult<UserDto> for Swagger-støtte
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _service.GetCurrentUserAsync(userId);

        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: 401);
    }

    // TODO: bør være ActionResult<List<UserDto>> for Swagger-støtte
    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _service.GetAllUsersAsync(currentUserId);

        return result.Success
            ? Ok(result.Data)
            : Problem(detail: result.Error, statusCode: 400);
    }

    // IActionResult: returnerer ingen data, bare sletter cookie
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




