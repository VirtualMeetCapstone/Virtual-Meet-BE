namespace GOCAP.Database;

[Table("RoomEvents")]
public class RoomEventEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public string? EventName { get; set; }
    public long EventDate { get; set; }
    public RoomEntity? Room { get; set; }
}
