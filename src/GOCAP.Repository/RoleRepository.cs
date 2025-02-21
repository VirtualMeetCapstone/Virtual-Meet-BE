namespace GOCAP.Repository;

[RegisterService(typeof(IRoleRepository))]
internal class RoleRepository(AppSqlDbContext context) : SqlRepositoryBase<RoleEntity>(context), IRoleRepository
{
}