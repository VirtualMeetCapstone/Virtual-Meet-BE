namespace GOCAP.Database;

[BsonCollection("CommentReactions")]
public class CommentReactionEntity : EntityMongoBase
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType? Type { get; set; }
    public CommentEntity? Comment { get; set; }
    public UserEntity? User { get; set; }
}
