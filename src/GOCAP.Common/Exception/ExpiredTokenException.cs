namespace GOCAP.Common;

public class ExpiredTokenException : ApiExceptionBase
{
    public ExpiredTokenException()
        : this("The token has been expired.")
    {
    }

    public ExpiredTokenException(string message)
        : base(message)
    {
        ErrorCode = (int)Common.ErrorCode.ExpiredAuthorizationToken;
        StatusCode = HttpStatusCode.Unauthorized;
    }
}
