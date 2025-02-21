namespace GOCAP.Repository.Intention;

public interface IPermissionRepository : ISqlRepositoryBase<PermissionEntity>
{
    Task<List<PermissionEntity>> GetUserPermissionsByUserIdAsync(Guid userId);
}
