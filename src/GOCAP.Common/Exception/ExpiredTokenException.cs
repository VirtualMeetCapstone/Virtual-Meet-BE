namespace GOCAP.Common;

public class ExpiredTokenException : ApiExceptionBase
{
    public ExpiredTokenException()
        : this("The token has been expired.")
    {
    }

    public ExpiredTokenException(String message)
        : base(message)
    {
        this.ErrorCode = (int)Common.ErrorCode.ExpiredAuthorizationToken;
        this.StatusCode = HttpStatusCode.Unauthorized;
    }
}
