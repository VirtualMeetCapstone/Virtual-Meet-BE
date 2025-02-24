namespace GOCAP.Common;

public class ForbiddenException : ApiExceptionBase
{
    public ForbiddenException()
        : this("403 Forbidden.")
    {
    }

    public ForbiddenException(Exception innerException)
        : this("403 Forbidden.", innerException)
    {
    }

    public ForbiddenException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = (int)Common.ErrorCode.Forbidden;
        StatusCode = HttpStatusCode.Forbidden;
    }
}
