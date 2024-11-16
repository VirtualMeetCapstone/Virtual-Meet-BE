namespace GOCAP.Common;

public class ResourceNotFoundException : ApiExceptionBase
{
    public ResourceNotFoundException()
        : this("The requested resource is not found.")
    {
    }

    public ResourceNotFoundException(String message)
        : base(message)
    {
        this.ErrorCode = (int)Common.ErrorCode.ResourceNotFound;
        this.StatusCode = HttpStatusCode.NotFound;
    }
}
