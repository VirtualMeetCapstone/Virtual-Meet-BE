namespace GOCAP.Database;

[Table("Notifications")]
public class NotificationEntity : EntitySqlBase
{
    [Required]
    public string? Message { get; set; }

    // Foreign Keys
    public Guid UserId { get; set; }

    // Relationships
    public UserEntity? User { get; set; }
}
