using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Server.Features.Sse;

[ApiController]
[Route("api/[controller]")]
public class SseController : ControllerBase
{
    private readonly SetConnectionManager _sseManager;

    public SseController(SetConnectionManager sseManager)
    {
        _sseManager = sseManager;
    }

    [HttpGet("stream")]
    [Authorize]
    public async Task Stream(CancellationToken cancellationToken)
    {

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
 

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _sseManager.AddConnection(userId, Response);


        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(30000, cancellationToken);
            await Response.WriteAsync($"event: ping\ndata: {{}}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}