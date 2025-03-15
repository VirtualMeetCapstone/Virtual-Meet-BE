namespace GOCAP.Domain;

public class MessageReaction : DateTrackingBase
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType? Type { get; set; }
}
