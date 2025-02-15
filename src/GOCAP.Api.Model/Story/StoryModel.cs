namespace GOCAP.Api.Model;

public class StoryModel
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Media? Media { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
    public DateTime ExpireTime { get; set; }
    public bool IsActive { get; set; } 
    public ICollection<StoryView> Views { get; set; } = [];
    public ICollection<StoryReaction> Reactions { get; set; } = [];
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
