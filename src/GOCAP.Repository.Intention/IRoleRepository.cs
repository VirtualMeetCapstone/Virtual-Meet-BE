namespace GOCAP.Repository.Intention;

public interface IRoleRepository : ISqlRepositoryBase<RoleEntity>
{
    Task<RoleEntity?> GetRoleByNameAsync(string name);
    Task<List<RoleEntity?>> GetRolesByUserIdAsync(Guid userId);
}