namespace GOCAP.Services.Intention;

public interface IPermissionService : IServiceBase<Permission>
{
    Task<List<Permission>> GetUserPermissionsByUserIdAsync(Guid userId);
}
