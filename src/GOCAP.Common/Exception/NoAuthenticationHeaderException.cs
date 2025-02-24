namespace GOCAP.Common;

public class NoAuthenticationHeaderException : ApiExceptionBase
{
    public NoAuthenticationHeaderException()
        : this("There is no authentication header in this request.")
    {
    }

    public NoAuthenticationHeaderException(string message)
        : base(message)
    {
        ErrorCode = (int)Common.ErrorCode.Unauthorized;
        StatusCode = HttpStatusCode.Unauthorized;
    }
}
