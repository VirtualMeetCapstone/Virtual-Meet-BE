namespace GOCAP.Common;

public class ResourceNotFoundException : ApiExceptionBase
{
    public ResourceNotFoundException()
        : this("The requested resource is not found.")
    {
    }

    public ResourceNotFoundException(string message)
        : base(message)
    {
        ErrorCode = (int)Common.ErrorCode.ResourceNotFound;
        StatusCode = HttpStatusCode.NotFound;
    }
}
