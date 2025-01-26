namespace GOCAP.Domain;

public class UserNotification : DateObjectBase
{
    public Guid UserId { get; set; }
    public User? Sender { get; set; }
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public Guid ReferenceId { get; set; }
    public bool IsRead { get; set; }
}
