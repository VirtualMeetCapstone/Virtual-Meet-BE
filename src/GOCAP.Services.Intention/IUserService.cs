namespace GOCAP.Services.Intention;

public interface IUserService : IServiceBase<User>
{
    Task<bool> IsEmailExists(string email);
    Task<User?> GetByEmail(string email);
}
