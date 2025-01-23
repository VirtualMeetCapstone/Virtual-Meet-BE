namespace GOCAP.Database;

[Table("RoomFavourites")]
public class RoomFavouriteEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
    public RoomEntity? Room { get; set; }
    public UserEntity? User { get; set; }
}
