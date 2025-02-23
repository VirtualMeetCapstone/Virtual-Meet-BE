namespace GOCAP.Repository;

[RegisterService(typeof(IUserRepository))]
internal class UserRepository(AppSqlDbContext context, IMapper _mapper, IBlobStorageService _blobStorageService) : SqlRepositoryBase<UserEntity>(context), IUserRepository
{
	private readonly AppSqlDbContext _context = context;

	public async Task<User?> GetByEmailAsync(string email)
	{
		var entity = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
		return _mapper.Map<User>(entity);
	}

	public async Task<UserCount> GetUserCountsAsync()
	{
		var counts = await _context.Users
		.GroupBy(u => 1)
		.Select(g => new UserCount
		{
			Total = g.Count(),
			Active = g.Count(u => u.Status == UserStatusType.Active),
			InActive = g.Count(u => u.Status == UserStatusType.InActive),
			Deleted = g.Count(u => u.Status == UserStatusType.Deleted)
		})
		.FirstOrDefaultAsync();

		return counts ?? new UserCount();
	}

	public async Task<bool> IsEmailExistsAsync(string email)
	{
		return await _context.Users.AnyAsync(user => user.Email == email);
	}

	public async Task<User> GetUserProfileAsync(Guid id)
	{
		var result = await _context.Users
									.AsNoTracking()
									.Where(user => user.Id == id)
									.Select(user => new User
									{
										Name = user.Name,
										Picture = string.IsNullOrEmpty(user.Picture) ?
												  null : JsonHelper.Deserialize<Media>(user.Picture),
										Bio = user.Bio
									})
									.FirstOrDefaultAsync()
									?? throw new ResourceNotFoundException($"User {id} was not found.");

		var count = await _context.UserFollows
									.AsNoTracking()
									.Where(f => f.FollowerId == id || f.FollowingId == id)
									.Select(f => new
									{
										FollowersCount = _context.UserFollows.Count(f => f.FollowingId == id),
										FollowingsCount = _context.UserFollows.Count(f => f.FollowerId == id),
										FriendsCount = _context.UserFollows
											.Count(f => f.FollowerId == id && f.FollowingId != id
												&& _context.UserFollows.Any(f2 => f2.FollowerId == f.FollowingId
												&& f2.FollowingId == id))
									})
									.FirstOrDefaultAsync()
									?? new
									{
										FollowersCount = 0,
										FollowingsCount = 0,
										FriendsCount = 0
									};

		result.FollowersCount = count.FollowersCount;
		result.FollowingsCount = count.FollowingsCount;
		result.FriendsCount = count.FriendsCount;
		return result;
	}

	public override async Task<bool> UpdateAsync(UserEntity entity)
	{
		var userEntity = await GetEntityByIdAsync(entity.Id);
		if (!string.IsNullOrEmpty(userEntity.Picture))
		{
			var media = JsonHelper.Deserialize<Media>(userEntity.Picture);
			await _blobStorageService.DeleteFilesByUrlsAsync([media?.Url]);
		}
		userEntity.Name = entity.Name;
		userEntity.Picture = entity.Picture;
		userEntity.Bio = entity.Bio;
		userEntity.LastModifyTime = entity.LastModifyTime;
		_context.Entry(userEntity).State = EntityState.Modified;
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<UserBlock?> GetBlockOrBlockedAsync(UserBlock model)
	{
		var list = await _context.UserBlocks.ToListAsync();
		var entity = await _context.UserBlocks.FirstOrDefaultAsync
											  (x => x.BlockedUserId == model.BlockedUserId
											   && x.BlockedByUserId == model.BlockedByUserId);

		//var userBlock = new UserBlock
		//{
		//	BlockedUserId = entity.BlockedUserId,
		//	BlockedByUserId = entity.BlockedByUserId
		//};
		return _mapper.Map<UserBlock>(entity);
	}
}
