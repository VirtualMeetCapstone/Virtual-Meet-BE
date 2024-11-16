namespace GOCAP.Domain;

public class Follow : DateObjectBase
{
    public Guid FollowerId { get; set; }

    public Guid FollowingId { get; set; }
    public User? Follower { get; set; }
    public User? Following { get; set; }
}
