namespace GOCAP.Repository;

public interface IUserRepository : ISqlRepositoryBase<User>
{
    Task<UserCount> GetUserCountsAsync();
    Task<bool> IsEmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User> GetUserProfileAsync(Guid id);
}
