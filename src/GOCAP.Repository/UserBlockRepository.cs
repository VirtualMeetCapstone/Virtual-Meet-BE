
namespace GOCAP.Repository;

[RegisterService(typeof(IUserBlockRepository))]
internal class UserBlockRepository
	(AppSqlDbContext context, IMapper _mapper) : SqlRepositoryBase<UserBlockEntity>(context), IUserBlockRepository
{
	private readonly AppSqlDbContext _context = context;
	public async Task<UserBlock?> GetBlockOrBlockedAsync(UserBlock model)
	{
		var entity = await _context.UserBlocks.FirstOrDefaultAsync
											  (x => x.BlockedUserId == model.BlockedUserId
											   && x.BlockedByUserId == model.BlockedByUserId);
		return _mapper.Map<UserBlock>(entity);
	}

	public async Task<List<UserBlock>> GetUserBlockAsync(Guid userId)
	{
		var entities = await _context.UserBlocks
			.Where(x => x.BlockedUserId == userId)
			.Include(x => x.BlockedUser)
			.ToListAsync();

		return _mapper.Map<List<UserBlock>>(entities);
	}
}
