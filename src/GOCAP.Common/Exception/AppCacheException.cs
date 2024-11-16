namespace GOCAP.Common;

public class AppCacheException : ApiExceptionBase
{
    public AppCacheException()
        : this("An error occurred when getting application cache.")
    {
    }

    public AppCacheException(String message)
        : base(message)
    {
    }

    public AppCacheException(String invalidParameterName, String message)
        : base(message)
    {
        this.InvalidParameterName = invalidParameterName;
    }

    public string? InvalidParameterName { get; set; }
}
