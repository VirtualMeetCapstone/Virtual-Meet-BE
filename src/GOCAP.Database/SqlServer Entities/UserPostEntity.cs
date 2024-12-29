namespace GOCAP.Database;

[Table("UserPosts")]
public class UserPostEntity : EntitySqlBase
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
    public UserEntity? User { get; set; }
}

