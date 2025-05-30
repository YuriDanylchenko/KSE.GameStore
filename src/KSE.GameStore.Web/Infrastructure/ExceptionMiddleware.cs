using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Responses;

namespace KSE.GameStore.Web.Infrastructure;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ServerException e)
        {
            _logger.LogWarning("Server exception {StatusCode} occurred while processing {Method} {Path}: {Message}",
                e.StatusCode, context.Request.Method, context.Request.Path, e.Message);
            await WriteErrorResponse(context, e.StatusCode, e.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected exception (500) while processing {Method} {Path}: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);
            await WriteErrorResponse(context, 500, ex.Message);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = message,
            Status = statusCode
        });
    }
}