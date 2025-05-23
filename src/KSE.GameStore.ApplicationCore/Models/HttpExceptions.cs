namespace KSE.GameStore.ApplicationCore.Models;

public class BadRequestException : ServerException
{
    public BadRequestException(string message) : base(message, 400) { }
}

public class UnauthorizedException : ServerException
{
    public UnauthorizedException(string message) : base(message, 401) { }
}

public class ForbiddenException : ServerException
{
    public ForbiddenException(string message) : base(message, 403) { }
}

public class NotFoundException : ServerException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class InternalServerErrorException : ServerException
{
    public InternalServerErrorException(string message) : base(message, 500) { }
}
