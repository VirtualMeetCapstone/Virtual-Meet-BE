namespace GOCAP.Domain;

public class StoryReaction : DateTrackingBase
{
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
    public Story? Story { get; set; }
    public User? User { get; set; }
}
