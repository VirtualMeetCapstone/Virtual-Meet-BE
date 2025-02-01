namespace GOCAP.Api.Model;

public class PostLikeModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
}
