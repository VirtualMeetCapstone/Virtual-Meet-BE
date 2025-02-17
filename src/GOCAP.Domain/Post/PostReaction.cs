namespace GOCAP.Domain;

public class PostReaction : DateTrackingBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
