namespace KSE.GameStore.Web.Infrastructure;
public class LoggerMiddleware(ILogger<LoggerMiddleware> logger) : IMiddleware
{
    private readonly ILogger<LoggerMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation("Received request: {Method} {Path} at {Date}", context.Request.Method, context.Request.Path, DateTime.UtcNow);
        await next(context);
        _logger.LogInformation("Request completed with status code: {StatusCode}", context.Response.StatusCode);
    }
}