namespace GOCAP.Domain;

public class StoryView : DateObjectBase
{
    public Guid StoryId { get; set; }
    public Guid ViewerId { get; set; }
}
