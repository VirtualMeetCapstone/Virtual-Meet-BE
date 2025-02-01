namespace GOCAP.Services.Intention;

public interface IUserService : IServiceBase<User>
{
    Task<List<UserNotification>> GetNotificationsByUserIdAsync(Guid userId);
    Task<bool> IsEmailExists(string email);
    Task<User?> GetByEmail(string email);
}
