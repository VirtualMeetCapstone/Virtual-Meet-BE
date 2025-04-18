namespace GOCAP.Api.Model;

public class MessageReactionCreationModel
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
}
