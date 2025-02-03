namespace GOCAP.Database;

[Table("UserStories")]
public class UserStoryEntity : EntitySqlBase
{
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
}

