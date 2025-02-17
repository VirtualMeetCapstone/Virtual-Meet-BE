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
        var entity = await GetEntityByIdAsync(id);
        var result = _mapper.Map<User>(entity);
        result.Picture = JsonHelper.Deserialize<Media>(entity.Picture);
        result.FollowersCount = await _context.UserFollows
                                            .AsNoTracking()
                                            .CountAsync(f => f.FollowingId == id);
        result.FollowingsCount = await _context.UserFollows
                                            .AsNoTracking()
                                            .CountAsync(f => f.FollowerId == id);
        result.FriendsCount = await _context.UserFollows
                                            .AsNoTracking()
                                            .Where(f => _context.UserFollows
                                            .Any(x => x.FollowerId == f.FollowingId && x.FollowingId ==                                       f.FollowerId))
                                            .CountAsync(f => f.FollowerId == id);
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
}
