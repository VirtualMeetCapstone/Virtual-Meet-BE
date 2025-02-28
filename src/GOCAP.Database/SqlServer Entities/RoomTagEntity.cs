namespace GOCAP.Database;

[Table("RoomTags")]
public class RoomTagEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public Guid TaggedUserId { get; set; }

    public RoomEntity? Room { get; set; }
    public UserEntity? TaggedUser { get; set; }

}

