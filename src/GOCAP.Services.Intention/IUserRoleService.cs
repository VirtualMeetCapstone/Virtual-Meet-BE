namespace GOCAP.Services.Intention;

public interface IUserRoleService : IServiceBase<UserRole>
{
    public Task<UserRole> AssignRoleToUser(UserRole domain);
}
