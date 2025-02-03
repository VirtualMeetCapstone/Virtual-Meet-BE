namespace GOCAP.Database;

[Table("UserFollows")]
public class UserFollowEntity : EntitySqlBase
{
    public Guid FollowerId { get; set; }
    public UserEntity? Follower { get; set; }

    public Guid FollowingId { get; set; }
    public UserEntity? Following { get; set; }
}
    