namespace GOCAP.Database;

[Table("Posts")]
public class PostEntity : EntitySqlBase
{
    [MaxLength(AppConstants.MaxLengthDescription)]
    public required string Content { get; set; }
    public string? Medias { get; set; }
    public PrivacyType Privacy { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public ICollection<PostReactionEntity> Reactions { get; set; } = [];
}

