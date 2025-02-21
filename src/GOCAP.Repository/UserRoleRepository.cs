namespace GOCAP.Repository;

[RegisterService(typeof(IUserRoleRepository))]
internal class UserRoleRepository(AppSqlDbContext context) : SqlRepositoryBase<UserRoleEntity>(context), IUserRoleRepository
{
}