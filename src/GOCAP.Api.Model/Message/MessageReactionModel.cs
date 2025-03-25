namespace GOCAP.Api.Model;

public class MessageReactionModel
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
    public long CreateTime { get; set; }
}
