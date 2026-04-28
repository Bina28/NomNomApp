using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Server.Features.Sse;

[ApiController]
[Route("api/[controller]")]
public class SseController : ControllerBase
{
    private readonly SetConnectionManager _sseManager;
    private readonly ILogger<SseController> _logger;

    public SseController(SetConnectionManager sseManager, ILogger<SseController> logger)
    {
        _sseManager = sseManager;
        _logger = logger;
    }

    [HttpGet("stream")]
    [Authorize]
    public async Task Stream(CancellationToken cancellationToken)
    {

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");


        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            Response.StatusCode = 401;
            return;
        }

        _logger.LogInformation("SSE connected: {UserId}", userId);
        _sseManager.AddConnection(userId, Response);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                
                await Task.Delay(30000, cancellationToken);
                await Response.WriteAsync($"event: ping\ndata: {{}}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        finally
        {
            _sseManager.RemoveConnection(userId);
            _logger.LogInformation("SSE disconnected: {UserId}", userId);
        }
    }
}