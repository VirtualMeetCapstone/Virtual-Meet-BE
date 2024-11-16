namespace GOCAP.Repository;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<bool> IsEmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
}
