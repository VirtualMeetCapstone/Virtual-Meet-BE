namespace GOCAP.Database;

[Table("RoomSettings")]
public class RoomSettingEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public string? SettingName { get; set; }
    public string? SettingValue { get; set; }
    public RoomEntity? Room { get; set; }
}
