namespace GOCAP.Database;

[Table("RoomHashTags")]
public class RoomHashTagEntity : EntitySqlBase
{   
    public Guid RoomId { get; set; }
    public RoomEntity? Room { get; set; }
    public Guid HashTagId { get; set; }
    public HashTagEntity? HashTag { get; set; }
}
