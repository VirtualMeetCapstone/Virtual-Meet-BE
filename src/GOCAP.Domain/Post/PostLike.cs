namespace GOCAP.Domain;

public class PostLike : DomainBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
}
