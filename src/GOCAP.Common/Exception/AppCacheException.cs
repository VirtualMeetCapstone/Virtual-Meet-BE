namespace GOCAP.Common;

public class AppCacheException : ApiExceptionBase
{
    public AppCacheException()
        : this("An error occurred when getting application cache.")
    {
    }

    public AppCacheException(string message)
        : base(message)
    {
    }

    public AppCacheException(string invalidParameterName, string message)
        : base(message)
    {
        InvalidParameterName = invalidParameterName;
    }

    public string? InvalidParameterName { get; set; }
}
