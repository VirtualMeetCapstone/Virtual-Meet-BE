namespace GOCAP.Repository;

[RegisterService(typeof(IUserRepository))]
internal class UserRepository(AppSqlDbContext context, IBlobStorageService _blobStorageService) : SqlRepositoryBase<UserEntity>(context), IUserRepository
{
    private readonly AppSqlDbContext _context = context;

    public async Task<UserEntity?> GetByEmailAsync(string email)
    => await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Email == email);
    public async Task<UserCount> GetUserCountsAsync()
    {
        var counts = await _context.Users
              .AsNoTracking()
            .Select(u => new
            {
                u.Status,
                u.IsDeleted
            })
            .GroupBy(u => 1)
            .Select(g => new UserCount
            {
                Total = g.Count(),
                Active = g.Count(u => u.Status == UserStatusType.Active),
                InActive = g.Count(u => u.Status == UserStatusType.InActive),
                Deleted = g.Count(u => u.IsDeleted),
                Banned = g.Count(u => u.Status == UserStatusType.Banned)
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
                                        Bio = user.Bio,
                                        Status = user.Status
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
        var userEntity = await GetByIdAsync(entity.Id, false);
        
        if (!string.IsNullOrEmpty(entity.Name))
        {
            userEntity.Name = entity.Name;
        }
        if (!string.IsNullOrEmpty(entity.Picture))
        {
            if (!string.IsNullOrEmpty(userEntity.Picture))
            {
                var media = JsonHelper.Deserialize<Media>(userEntity.Picture);
                await _blobStorageService.DeleteFilesByUrlsAsync([media?.Url]);
            }
            userEntity.Picture = entity.Picture;
        }
        if (entity.Bio != null)
        {
            userEntity.Bio = entity.Bio;
        }
        userEntity.LastModifyTime = entity.LastModifyTime;
        _context.Entry(userEntity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }

    public override async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity = await _context.Users.FindAsync(id)
                    ?? throw new ResourceNotFoundException($"User {id} was not found.");

        entity.IsDeleted = true;
        int rowsAffected = await _context.SaveChangesAsync();

        return rowsAffected;
    }

    public async Task<List<User>> SearchUsersAsync(string userName, int limit)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => EF.Functions.Collate(u.Name, "Latin1_General_CI_AI").Contains(userName)
                    && !u.IsDeleted)
            .OrderBy(u => u.Name)
            .Take(limit)
            .Select(user => new User
            {
                Id = user.Id,
                Name = user.Name,
                Picture = string.IsNullOrEmpty(user.Picture) ? null
                        : JsonHelper.Deserialize<Media>(user.Picture)
            })
            .ToListAsync();
    }
}
