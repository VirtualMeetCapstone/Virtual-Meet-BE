namespace GOCAP.Api.Model;

public class StoryUserModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public string? Content { get; set; }
    public Media? Media { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
    public long ExpireTime { get; set; }
    public bool IsActive { get; set; }
}
