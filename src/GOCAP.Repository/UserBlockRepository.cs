
namespace GOCAP.Repository;

[RegisterService(typeof(IUserBlockRepository))]
internal class UserBlockRepository
	(AppSqlDbContext context, IMapper _mapper) : SqlRepositoryBase<UserBlockEntity>(context), IUserBlockRepository
{
	private readonly AppSqlDbContext _context = context;
	public async Task<UserBlock?> GetBlockOrBlockedAsync(UserBlock domain)
	{
		var entity = await _context.UserBlocks.AsNoTracking().FirstOrDefaultAsync
											  (x => x.BlockedUserId == domain.BlockedUserId
											   && x.BlockedByUserId == domain.BlockedByUserId);
		return _mapper.Map<UserBlock>(entity);
	}

    public async Task<List<UserBlock>> GetUserBlocksAsync(Guid userId)
    {
        var entities = await _context.UserBlocks
            .AsNoTracking()
            .Where(u => u.BlockedByUserId == userId)
            .Join(
                _context.Users.AsNoTracking(),
                block => block.BlockedUserId,
                user => user.Id,
                (block, user) => new UserBlock
                {
                    Id = block.Id,
                    Name = user.Name,
                    Picture = string.IsNullOrEmpty(user.Picture) ?
                              null : JsonHelper.Deserialize<Media>(user.Picture),
                })
            .ToListAsync();

        return entities!;
    }

}
