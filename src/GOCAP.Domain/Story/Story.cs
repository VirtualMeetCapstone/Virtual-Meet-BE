namespace GOCAP.Domain;

public class Story : DateTrackingBase
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Media? Media { get; set; }
    public MediaUpload? MediaUpload { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
    public long ExpireTime { get; set; } = DateTime.UtcNow.AddHours(24).Ticks;
    public bool IsActive { get; set; } = true;
    public ICollection<StoryView> Views { get; set; } = [];
    public ICollection<StoryReaction> Reactions { get; set; } = [];
}
