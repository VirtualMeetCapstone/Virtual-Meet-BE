namespace GOCAP.Database;

[Table("UserPostLikes")]
public class UserPostLikeEntity : EntitySqlBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public UserPostEntity? Post { get; set; }
    public UserEntity? User { get; set; }
}
