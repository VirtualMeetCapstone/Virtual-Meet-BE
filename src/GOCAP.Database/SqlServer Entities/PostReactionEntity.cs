namespace GOCAP.Database;

[Table("PostReactions")]
public class PostReactionEntity : EntitySqlBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType? Type { get; set; }
    public PostEntity? Post { get; set; }
    public UserEntity? User { get; set; }
}
