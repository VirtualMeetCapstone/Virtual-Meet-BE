
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

	public async Task<List<UserBlock>> GetUserBlocksAsync(Guid userId)
	{
		var entities = await _context.UserBlocks
			.AsNoTracking()
			.Where(u => u.BlockedUserId == userId)
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
