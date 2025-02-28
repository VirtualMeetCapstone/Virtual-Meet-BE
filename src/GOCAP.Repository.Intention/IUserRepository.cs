namespace GOCAP.Repository;

public interface IUserRepository : ISqlRepositoryBase<UserEntity>
{
    Task<UserCount> GetUserCountsAsync();
    Task<bool> IsEmailExistsAsync(string email);
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<User> GetUserProfileAsync(Guid id);
	Task<List<User>> SearchUsersAsync(string userName, int limit);

}
