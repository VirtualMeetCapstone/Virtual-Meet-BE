namespace GOCAP.Api.Model;

public class FollowCreationModel
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}