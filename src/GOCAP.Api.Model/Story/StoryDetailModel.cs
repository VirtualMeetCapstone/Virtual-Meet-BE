namespace GOCAP.Api.Model;

public class StoryDetailModel
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public StoryUserModel? User { get; set; }
    public Media? Media { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
    public long ExpireTime { get; set; }
    public bool IsActive { get; set; }
    public ICollection<StoryView> Views { get; set; } = [];
    public ICollection<StoryReaction> Reactions { get; set; } = [];
}
