namespace GOCAP.Repository.Intention;

public interface IUserNotificationRepository : ISqlRepositoryBase<UserNotification>
{
    Task<List<UserNotification>> GetNotificationsByUserIdAsync(Guid userId);
}
