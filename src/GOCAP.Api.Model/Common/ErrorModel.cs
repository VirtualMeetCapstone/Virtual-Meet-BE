using System.Diagnostics;

namespace GOCAP.Api.Model;

public class ErrorModel
{
    /// <summary>
    /// The trace id of response
    /// </summary>
    public string TraceId { get; set; } = Activity.Current?.TraceId.ToString() ?? string.Empty;

    /// <summary>
    /// The timestamp of current response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The error code of current exception
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// The error message of current exception
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The error details of current exception
    /// </summary>
    public List<string>? ErrorDetails { get; set; }
}
