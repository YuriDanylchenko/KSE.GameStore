using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KSE.GameStore.ApplicationCore.Infrastructure;

public class LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
{
    private readonly ILogger<LoggerMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation("Received request: {Method} {Path} at {Date}", context.Request.Method,
            context.Request.Path, DateTime.UtcNow);
        await next(context);
        _logger.LogInformation("Request completed with status code: {StatusCode}", context.Response.StatusCode);
    }
}