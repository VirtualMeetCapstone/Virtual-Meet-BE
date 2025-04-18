namespace GOCAP.Services.Intention;

public interface INotificationService : IServiceBase<Notification>
{
    Task<Notification> HandleNotificationEvent(NotificationEvent notificationEvent);
    Task<QueryResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, QueryInfo queryInfo);
    Task<OperationResult> MarkAsReadAsync(Guid userId, Guid notificationId);
}
