namespace GOCAP.Common;

public class OperationResult
{
    public bool Success {  get; set; }
    public string Message { get; set; }

    public OperationResult(bool success, string message = "")
    {
        if (!success)
        {
            message = "Operation Failed";
        }
        Success = success;
        Message = message;
    }
}
