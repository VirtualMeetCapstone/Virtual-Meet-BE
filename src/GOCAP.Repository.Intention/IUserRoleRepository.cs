namespace GOCAP.Repository.Intention;

public interface IUserRoleRepository : ISqlRepositoryBase<UserRoleEntity>
{
    public Task<UserRoleEntity> AssignRoleToUser(UserRoleEntity entity);
}