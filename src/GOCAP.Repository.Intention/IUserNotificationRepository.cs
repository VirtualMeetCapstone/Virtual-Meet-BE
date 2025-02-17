namespace GOCAP.Repository.Intention;

public interface IUserNotificationRepository : ISqlRepositoryBase<UserNotificationEntity>
{
    Task<List<UserNotification>> GetNotificationsByUserIdAsync(Guid userId);
}
