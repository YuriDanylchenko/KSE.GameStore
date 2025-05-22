using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Responses;

namespace KSE.GameStore.Web.Infrastructure;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ServerException e)
        {
            _logger.LogWarning("Server exception {StatusCode} occurred while processing {Method} {Path}: {Message}", e.StatusCode, context.Request.Method, context.Request.Path, e.Message);
            await WriteErrorResponse(context, e.StatusCode, e.Message);
        }
        catch (NullReferenceException nullReferenceException)
        {
            _logger.LogWarning("NullReferenceException {StatusCode} occurred while processing {Method} {Path}: {Message}", 404, context.Request.Method, context.Request.Path, nullReferenceException.Message);
            await WriteErrorResponse(context, 404, nullReferenceException.Message);
        }
        catch (System.Exception ex)
        {
                _logger.LogError("Unexpected exception {StatusCode} occurred while processing {Method} {Path}: {Message}", 500, context.Request.Method, context.Request.Path, ex.Message);
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
