
namespace GOCAP.Repository;

[RegisterService(typeof(IRoleRepository))]
internal class RoleRepository(AppSqlDbContext context) : SqlRepositoryBase<RoleEntity>(context), IRoleRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<RoleEntity?> GetRoleByNameAsync(string name)
    => await _context.Roles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Name == name);

    public async Task<List<RoleEntity?>> GetRolesByUserIdAsync(Guid userId)
    => await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToListAsync();
}