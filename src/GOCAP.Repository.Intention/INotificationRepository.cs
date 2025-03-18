namespace GOCAP.Repository.Intention;

public interface INotificationRepository : IMongoRepositoryBase<NotificationEntity>
{
    Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId);
}
