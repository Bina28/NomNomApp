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
        return result.Ok ?
             Ok(new { Token = result.Data })
             : Unauthorized(new { result.Error });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _service.RegisterAsync(request);
        return result.Ok ?
            Ok(new { Token = result.Data })
            : BadRequest(new { result.Error });
    }
}




