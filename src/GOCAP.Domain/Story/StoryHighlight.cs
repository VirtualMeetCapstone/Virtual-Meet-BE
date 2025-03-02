namespace GOCAP.Domain;

public class StoryHighlight : DateTrackingBase
{
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public Guid? PrevStoryId { get; set; }
    public Guid? NextStoryId { get; set; }
}
