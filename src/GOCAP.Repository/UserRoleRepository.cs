namespace GOCAP.Repository;

[RegisterService(typeof(IUserRoleRepository))]
internal class UserRoleRepository(AppSqlDbContext context) : SqlRepositoryBase<UserRoleEntity>(context), IUserRoleRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<UserRoleEntity> AssignRoleToUser(UserRoleEntity entity)
    {
        await ValidateAsync(entity);
        await _context.UserRoles.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task ValidateAsync(UserRoleEntity entity)
    {
        if (!await _context.Roles.AsNoTracking().AnyAsync(r => r.Id == entity.RoleId))
        {
            throw new ResourceNotFoundException($"Role {entity.RoleId} was not found.");
        }
        if (!await _context.Users.AsNoTracking().AnyAsync(u => u.Id == entity.UserId))
        {
            throw new ResourceNotFoundException($"User {entity.User} was not found.");
        }
    }
}