namespace GOCAP.Database;

[Table("UserNotifications")]
public class UserNotificationEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public string NotificationContent { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public long CreatedAt { get; set; }
    public UserEntity? User { get; set; }
}
