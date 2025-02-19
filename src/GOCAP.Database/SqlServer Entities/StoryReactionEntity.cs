namespace GOCAP.Database;

[Table("StoryReactions")]
public class StoryReactionEntity : EntitySqlBase
{
    public Guid StoryId { get; set; }
    public StoryEntity? Story { get; set; }
    public ReactionType? Type { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
}
