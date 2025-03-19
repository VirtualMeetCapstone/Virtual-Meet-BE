namespace GOCAP.Repository.Intention;

public interface INotificationRepository : IMongoRepositoryBase<NotificationEntity>
{
    Task<QueryResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, QueryInfo queryInfo);
    Task<bool> MarkAsReadAsync(Guid userId, Guid notificationId);
}
