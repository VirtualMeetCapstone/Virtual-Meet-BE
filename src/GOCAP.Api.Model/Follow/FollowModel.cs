namespace GOCAP.Api.Model;

public class FollowModel
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public long CreateTime { get; set; }
}