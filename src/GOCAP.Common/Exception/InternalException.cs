namespace GOCAP.Common;

public class InternalException : ApiExceptionBase
{
    public InternalException()
        : this("An unexpected error occurred.")
    {
    }

    public InternalException(String message)
        : base(message)
    {
        this.ErrorCode = (int)Common.ErrorCode.InternalError;
    }
}
