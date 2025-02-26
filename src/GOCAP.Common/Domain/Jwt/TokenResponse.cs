namespace GOCAP.Common;

public class TokenResponse
{
    public OperationResult Result { get; set; } = new OperationResult(true);
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
