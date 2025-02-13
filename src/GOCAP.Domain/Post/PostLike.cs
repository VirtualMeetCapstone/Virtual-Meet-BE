namespace GOCAP.Domain;

public class PostLike : DateObjectBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
