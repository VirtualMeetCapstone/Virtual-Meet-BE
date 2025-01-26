namespace GOCAP.Domain;

public class Follow : DomainBase
{
    
    public Guid FollowerId { get; set; } // If of the follower
    public Guid FollowingId { get; set; } // Id of the followed
    public long CreateTime { get; set; }
}
