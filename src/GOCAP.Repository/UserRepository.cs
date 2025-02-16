namespace GOCAP.Repository;

[RegisterService(typeof(IUserRepository))]
internal class UserRepository(AppSqlDbContext context,
    IMapper mapper,
    IBlobStorageService _blobStorageService
    ) : SqlRepositoryBase<User, UserEntity>(context, mapper), IUserRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;

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
                                            .Any(x => x.FollowerId == f.FollowingId && x.FollowingId == f.FollowerId))
                                            .CountAsync(f => f.FollowerId == id);
        return result;
    }

    public override async Task<bool> UpdateAsync(Guid id, User domain)
    {
        var entity = await GetEntityByIdAsync(id);
        if (domain.PictureUpload != null)
        {
            var picture = await _blobStorageService.UploadFileAsync(domain.PictureUpload);
            entity.Picture = JsonHelper.Serialize(picture);

            if (!string.IsNullOrEmpty(entity.Picture))
            {
                var media = JsonHelper.Deserialize<Media>(entity.Picture);
                await _blobStorageService.DeleteFilesByUrlsAsync([media?.Url]);
            }
        }

        entity.Name = domain.Name;
        entity.Bio = domain.Bio;
        entity.LastModifyTime = domain.LastModifyTime;
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}
