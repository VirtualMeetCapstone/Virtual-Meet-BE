namespace GOCAP.Common;

public class ParameterInvalidException : ApiExceptionBase
{
    public ParameterInvalidException()
        : this("An error occurred when getting application cache.")
    {
    }

    public ParameterInvalidException(String message)
        : base(message)
    {
        this.ErrorCode = (int)Common.ErrorCode.InvalidParameters;
        this.StatusCode = HttpStatusCode.BadRequest;
    }
}
