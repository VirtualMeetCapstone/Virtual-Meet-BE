namespace GOCAP.Api.Model;

public class StoryHightLightModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public Guid? PrevStoryId { get; set; }
    public Guid? NextStoryId { get; set; }
}
