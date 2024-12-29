namespace GOCAP.Database;

[Table("UserStories")]
public class UserStoryEntity : EntitySqlBase
{
    public string? StoryContent { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
    public UserEntity? User { get; set; }
}

