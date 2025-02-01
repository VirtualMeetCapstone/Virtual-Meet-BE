namespace GOCAP.Database;

[Table("UserNotifications")]
public class UserNotificationEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public required string Content { get; set; }
    public NotificationType Type { get; set; }
    public Guid ReferenceId {  get; set; }
    public bool IsRead { get; set; }
    [Required]
    public long CreateTime { get; set; }
    [Required]
    public long LastModifyTime { get; set; }
    public UserEntity? User { get; set; }
}
