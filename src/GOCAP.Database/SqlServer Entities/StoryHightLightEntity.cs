namespace GOCAP.Database;

[Table("StoryHightLights")]
public class StoryHightLightEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public UserEntity? User { get; set; }
    public StoryEntity? Story { get; set; }
    public Guid? PrevStoryId { get; set; }
    public Guid? NextStoryId { get; set; }
    public StoryHightLightEntity? PrevStory { get; set; }
    public StoryHightLightEntity? NextStory { get; set; }
}
