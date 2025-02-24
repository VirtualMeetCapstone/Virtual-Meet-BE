namespace GOCAP.Common;

public class ResourceDuplicatedException : ApiExceptionBase
{
    public ResourceDuplicatedException()
        : this("The resource is duplicated.")
    {
    }

    public ResourceDuplicatedException(string message) : base(message)
    {
        ErrorCode = (int)Common.ErrorCode.ResourceDuplicated;
        StatusCode = HttpStatusCode.Conflict;
    }
}
