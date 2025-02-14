namespace GOCAP.Database;

public class StoryViewEntity : EntitySqlBase
{
    public Guid StoryId { get; set; }
    public StoryEntity? Story { get; set; }

    public Guid ViewerId { get; set; }
    public UserEntity? Viewer { get; set; }
}