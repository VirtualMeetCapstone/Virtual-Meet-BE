namespace GOCAP.Database;

[Table("RoomTags")]
public class RoomTagEntity : EntitySqlBase
{
    public string? TagName { get; set; }
    public Guid RoomId { get; set; }
    public RoomEntity? Room { get; set; }
}
