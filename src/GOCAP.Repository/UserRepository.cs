using AutoMapper;

namespace GOCAP.Repository;

[RegisterService(typeof(IUserRepository))]
internal class UserRepository(
    GoCapMsSqlDbContext _context, IMapper _mapper) : RepositoryBase<User, UserEntity>(_context, _mapper), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }
        var entity = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        return _mapper.Map<User>(entity);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        return await _context.Users.AnyAsync(user => user.Email == email);
    }
}
