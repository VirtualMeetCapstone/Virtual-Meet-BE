namespace GOCAP.Repository;

[RegisterService(typeof(IUserBlockRepository))]
internal class UserBlockRepository
	(AppSqlDbContext context, IMapper _mapper) : SqlRepositoryBase<UserBlockEntity>(context), IUserBlockRepository
{
	private readonly AppSqlDbContext _context = context;
	public async Task<UserBlock?> GetBlockOrBlockedAsync(UserBlock model)
	{
		var list = await _context.UserBlocks.ToListAsync();
		var entity = await _context.UserBlocks.FirstOrDefaultAsync
											  (x => x.BlockedUserId == model.BlockedUserId
											   && x.BlockedByUserId == model.BlockedByUserId);
		return _mapper.Map<UserBlock>(entity);
	}
}
