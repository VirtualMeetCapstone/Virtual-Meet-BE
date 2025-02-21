namespace GOCAP.Repository;

[RegisterService(typeof(IPermissionRepository))]
internal class PermissionRepository(AppSqlDbContext context) : SqlRepositoryBase<PermissionEntity>(context), IPermissionRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<List<PermissionEntity>> GetUserPermissionsByUserIdAsync(Guid userId)
    => await ( from ur in _context.UserRoles
               join r in _context.Roles on ur.RoleId equals r.Id
               join rp in _context.RolePermissions on r.Id equals rp.RoleId
               join p in _context.Permissions on rp.PermissionId equals p.Id
               where ur.UserId == userId
               select p
             ).Distinct()
              .ToListAsync();
    
}