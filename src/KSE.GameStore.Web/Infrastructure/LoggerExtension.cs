namespace KSE.GameStore.Web.Infrastructure;

public static class LoggerExtension
{
    public static void LogNotFound(this ILogger logger, string path, string? additionalInfo = null)
    {
        if (string.IsNullOrEmpty(additionalInfo))
        {
            logger.LogWarning("404 Not Found: Path '{Path}' was not found.", path);
        }
        else
        {
            logger.LogWarning("404 Not Found: Path '{Path}' was not found. Info: {Info}", path, additionalInfo);
        }
    }

    public static void LogServerError(this ILogger logger, string path, Exception exception, string? additionalInfo = null)
    {
        if (string.IsNullOrEmpty(additionalInfo))
        {
            logger.LogError(exception, "500 Internal Server Error: Path '{Path}' encountered an error. Message: {}", path, exception);
        }
        else
        {
            logger.LogError(exception, "500 Internal Server Error: Path '{Path}' encountered an error. Message: {}. Info: {Info}", path, exception, additionalInfo);
        }
    }
}

