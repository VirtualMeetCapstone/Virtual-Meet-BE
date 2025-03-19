namespace GOCAP.Services.Intention;

public interface INotificationService : IServiceBase<Notification>
{
    Task HandleNotificationEvent(NotificationEvent notificationEvent);
    Task<QueryResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, QueryInfo queryInfo);
    Task<OperationResult> MarkAsReadAsync(Guid userId, Guid notificationId);
}
