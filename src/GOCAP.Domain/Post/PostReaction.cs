namespace GOCAP.Domain;

public class PostReaction : DateTrackingBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType? Type { get; set; }
    public Post? Post { get; set; }
    public User? User { get; set; }
}
