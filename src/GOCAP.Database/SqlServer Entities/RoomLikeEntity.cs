namespace GOCAP.Database;

[Table("RoomLikes")]
public class RoomLikeEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
    public RoomEntity? Room { get; set; }
    public UserEntity? User { get; set; }
}
