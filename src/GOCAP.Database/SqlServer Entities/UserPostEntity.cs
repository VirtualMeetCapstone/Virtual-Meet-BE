namespace GOCAP.Database;

[Table("UserPosts")]
public class UserPostEntity : EntitySqlBase
{
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public required string Content { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public ICollection<UserPostLikeEntity> Likes { get; set; } = [];
}

