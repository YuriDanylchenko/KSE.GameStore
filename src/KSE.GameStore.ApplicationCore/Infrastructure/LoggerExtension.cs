using Microsoft.Extensions.Logging;

namespace KSE.GameStore.ApplicationCore.Infrastructure;

public static class LoggerExtension
{
    public static void LogNotFound(this ILogger logger, string path)
    {
        logger.LogWarning("404 Not Found: Path '{Path}' was not found.", path);
    }

    public static void LogNotFound(this ILogger logger, string path, string additionalInfo)
    {
        logger.LogWarning("404 Not Found: Path '{Path}' was not found. Info: {Info}", path, additionalInfo);
    }

    public static void LogServerError(this ILogger logger, string path, Exception exception)
    {
        logger.LogError(exception, "500 Internal Server Error: Path '{Path}' encountered an error. Message: {}", path, exception);
    }

    public static void LogServerError(this ILogger logger, string path, Exception exception, string additionalInfo)
    {
        logger.LogError(exception, "500 Internal Server Error: Path '{Path}' encountered an error. Message: {}. Info: {Info}", path, exception, additionalInfo);
    }
}