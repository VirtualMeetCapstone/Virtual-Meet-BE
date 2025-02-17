namespace GOCAP.Domain;

public class StoryView : DateTrackingBase
{
    public Guid StoryId { get; set; }
    public Guid ViewerId { get; set; }
}
