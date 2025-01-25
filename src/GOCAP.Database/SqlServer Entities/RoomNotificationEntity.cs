namespace GOCAP.Database;

[Table("RoomNotifications")]
public class RoomNotificationEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public string? NotificationContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsGlobal { get; set; }
    public RoomEntity? Room { get; set; }
}
