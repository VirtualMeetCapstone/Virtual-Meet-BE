namespace GOCAP.Api.Model;

public class StoryViewModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid ViewerId { get; set; }
}
