namespace GOCAP.Database;

[Table("Follows")]
public class FollowEntity : EntitySqlBase
{
    public Guid FollowerId { get; set; }
    public UserEntity? Follower { get; set; }

    public Guid FollowingId { get; set; }
    public UserEntity? Following { get; set; }
    public long CreateTime { get; set; }
}
    