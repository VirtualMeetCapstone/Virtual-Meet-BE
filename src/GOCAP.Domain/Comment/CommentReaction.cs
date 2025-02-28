namespace GOCAP.Domain;

public class CommentReaction : DateTrackingBase
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
}
