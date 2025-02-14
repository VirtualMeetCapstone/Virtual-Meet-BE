namespace GOCAP.Database;

[Table("Stories")]
public class StoryEntity : EntitySqlBase
{
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public string? Media { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
    [NotMapped]
    public DateTime ExpireTime => DateTimeOffset.FromUnixTimeSeconds(CreateTime).UtcDateTime.AddHours(24);
    public bool IsActive { get; set; } = true;
    public ICollection<StoryViewEntity> Views { get; set; } = [];
    public ICollection<StoryReactionEntity> Reactions { get; set; } = [];
}

