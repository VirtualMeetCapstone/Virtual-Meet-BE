namespace GOCAP.Api.Model;

public class PostReactionModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
