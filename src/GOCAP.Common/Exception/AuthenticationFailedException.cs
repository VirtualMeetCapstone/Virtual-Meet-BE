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

    public AuthenticationFailedException(String message, Exception? innerException = null)
        : base(message, innerException)
    {
        this.ErrorCode = (int)Common.ErrorCode.Unauthorized;
        this.StatusCode = HttpStatusCode.Unauthorized;
    }
}
