namespace GOCAP.Database;

[Table("Stories")]
public class StoryEntity : EntitySqlBase
{
    [MaxLength(AppConstants.MaxLengthDescription)]
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public string? Media { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
    public long ExpireTime { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<StoryViewEntity> Views { get; set; } = [];
    public ICollection<StoryReactionEntity> Reactions { get; set; } = [];
}

