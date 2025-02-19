namespace GOCAP.Api.Model;

public class StoryHightLightCreationModel
{
    public Guid UserId { get; set; }
    public Guid? PrevStoryId { get; set; }
    public Guid? NextStoryId { get; set; }
}
