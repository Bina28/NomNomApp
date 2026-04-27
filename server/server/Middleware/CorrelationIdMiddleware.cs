using Serilog.Context;

namespace Server.Middleware;

public class CorrelationIdMiddleware
{
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger, RequestDelegate requestDelegate)
    {
        _logger = logger;
        _next = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);

        }

    }
}
