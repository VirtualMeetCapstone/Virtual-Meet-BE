namespace GOCAP.Common;

public class ResourceDuplicatedException : ApiExceptionBase
{
    public ResourceDuplicatedException()
        : this("The resource is duplicated.")
    {
    }

    public ResourceDuplicatedException(String message) : base(message)
    {
        this.ErrorCode = (int)Common.ErrorCode.ResourceDuplicated;
        this.StatusCode = HttpStatusCode.Conflict;
    }
}
