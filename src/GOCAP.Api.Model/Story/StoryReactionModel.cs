namespace GOCAP.Api.Model;

public class StoryReactionModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
}
