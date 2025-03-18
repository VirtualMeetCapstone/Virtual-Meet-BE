namespace GOCAP.Api.Model;

public class NotificationModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public ReferenceNotificationModel? Sender { get; set; }
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
}

public class ReferenceNotificationModel
{
    public string Name { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
}