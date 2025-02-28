namespace GOCAP.Domain;

public class RefreshToken : DateTrackingBase
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public long ExpiresAt { get; set; }
    public long? RevokedAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}

