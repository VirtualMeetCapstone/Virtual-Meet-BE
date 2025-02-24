namespace GOCAP.Common;

public class ParameterInvalidException : ApiExceptionBase
{
    public ParameterInvalidException()
        : this("An error occurred when getting application cache.")
    {
    }

    public ParameterInvalidException(string message)
        : base(message)
    {
        ErrorCode = (int)Common.ErrorCode.InvalidParameters;
        StatusCode = HttpStatusCode.BadRequest;
    }
}
