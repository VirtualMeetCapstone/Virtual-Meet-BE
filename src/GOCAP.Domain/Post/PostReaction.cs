namespace GOCAP.Domain;

public class PostReaction : DateObjectBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
