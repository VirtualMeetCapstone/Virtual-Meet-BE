namespace GOCAP.Database;

[BsonCollection("MessageReactions")]
public class MessageReactionEntity : EntityMongoBase
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType? Type { get; set; }
}
