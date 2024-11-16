namespace GOCAP.Common;

public class NoAuthenticationHeaderException : ApiExceptionBase
{
    public NoAuthenticationHeaderException()
        : this("There is no authentication header in this request.")
    {
    }

    public NoAuthenticationHeaderException(String message)
        : base(message)
    {
        this.ErrorCode = (int)Common.ErrorCode.Unauthorized;
        this.StatusCode = HttpStatusCode.Unauthorized;
    }
}
