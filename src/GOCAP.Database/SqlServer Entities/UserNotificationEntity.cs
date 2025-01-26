namespace GOCAP.Database;

[Table("UserNotifications")]
public class UserNotificationEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public Guid ReferenceId {  get; set; }
    public bool IsRead { get; set; }
    [Required]
    public long CreateTime { get; set; }
    [Required]
    public long LastModifyTime { get; set; }
    public UserEntity? User { get; set; }
}
