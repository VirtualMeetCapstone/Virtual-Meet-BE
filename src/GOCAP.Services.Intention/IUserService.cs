namespace GOCAP.Services.Intention;

public interface IUserService : IServiceBase<User>
{
    Task<UserCount> GetUserCountsAsync();
    Task<List<UserNotification>> GetNotificationsByUserIdAsync(Guid userId);
    Task<User> GetUserProfileAsync(Guid id);
    Task<bool> IsEmailExists(string email);
    Task<User?> GetByEmail(string email);
    Task<List<User>> SearchUsersAsync(string userName, int limit);
}
