namespace GOCAP.Repository;

[RegisterService(typeof(IRolePermissionRepository))]
internal class RolePermissionRepository(AppSqlDbContext context) : SqlRepositoryBase<RolePermissionEntity>(context), IRolePermissionRepository
{
}