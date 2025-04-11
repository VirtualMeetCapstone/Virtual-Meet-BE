using GOCAP.Database;

namespace GOCAP.Services.Intention;

public interface IUserService : IServiceBase<User>
{
    Task<UserCount> GetUserCountsAsync();
    Task<User> GetUserProfileAsync(Guid id);
    Task<bool> IsEmailExists(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> SearchUsersAsync(string userName, int limit);
    Task AddOrUpdateUserVipAsync(Guid userId, string level, DateTime? expireAt = null);
    Task<UserVip> GetUserVipLevelAsync(Guid userId);
}
