namespace GOCAP.Common;

public class AuthenticationFailedException : ApiExceptionBase
{
    public AuthenticationFailedException()
        : this("401 Unauthorized.")
    {
    }

    public AuthenticationFailedException(Exception innerException)
        : this("401 Unauthorized.", innerException)
    {
    }

    public AuthenticationFailedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = (int)Common.ErrorCode.Unauthorized;
        StatusCode = HttpStatusCode.Unauthorized;
    }
}
