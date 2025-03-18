namespace GOCAP.Repository;

[RegisterService(typeof(INotificationRepository))]
internal class NotificationRepository
    (AppMongoDbContext context) : MongoRepositoryBase<NotificationEntity>(context), INotificationRepository
{
    public Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}