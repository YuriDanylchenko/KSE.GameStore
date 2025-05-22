namespace KSE.GameStore.ApplicationCore.Models;

public class ServerException : System.Exception
{
    public ServerException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }  
}