namespace GOCAP.Common;

public class InvalidTokenException : ApiExceptionBase
{
    public InvalidTokenException()
        : this("The token is invalid.")
    {
    }

    public InvalidTokenException(String message)
        : base(message)
    {
        ErrorCode = (int)Common.ErrorCode.InvalidAuthorizationToken;
    }
}
