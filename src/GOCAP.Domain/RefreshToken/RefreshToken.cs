namespace GOCAP.Domain;

public class RefreshToken : DateTrackingBase
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}

