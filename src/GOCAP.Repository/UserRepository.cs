namespace GOCAP.Repository;

[RegisterService(typeof(IUserRepository))]
internal class UserRepository(AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<User, UserEntity>(context, mapper), IUserRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<User?> GetByEmailAsync(string email)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        return _mapper.Map<User>(entity);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(user => user.Email == email);
    }
}
