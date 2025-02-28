namespace GOCAP.Repository.Intention;

public interface IUserRoleRepository : ISqlRepositoryBase<UserRoleEntity>
{
    public Task<UserRoleEntity> AssignRoleToUserAsync(UserRoleEntity entity);
}