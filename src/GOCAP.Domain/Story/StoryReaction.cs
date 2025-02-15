namespace GOCAP.Domain;

public class StoryReaction : DateObjectBase
{
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
}
