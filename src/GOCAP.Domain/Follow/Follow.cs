namespace GOCAP.Domain;

public class Follow : DateObjectBase
{ 
    public Guid FollowerId { get; set; } // If of the follower
    public Guid FollowingId { get; set; } // Id of the followed
}
