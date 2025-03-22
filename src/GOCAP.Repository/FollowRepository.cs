

namespace GOCAP.Repository;

[RegisterService(typeof(IFollowRepository))]
internal class FollowRepository
    (AppSqlDbContext context)
    : SqlRepositoryBase<UserFollowEntity>(context), IFollowRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<UserFollowEntity?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId)
    {
        var entity = await _context.UserFollows
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync
                                                (f => f.FollowerId == followerId
                                                 && f.FollowingId == followingId);
        return entity;
    }

    public async Task<List<Guid>> GetFollowersByUserIdAsync(Guid userId)
    => await _context.UserFollows
                        .AsNoTracking()
                        .Where(f => f.FollowingId == userId)
                        .Select(f => f.FollowerId)
                        .ToListAsync();

    public async Task<bool> IsFollowingAsync(Follow domain)
    => await _context.UserFollows
        .AsNoTracking()
        .AnyAsync(f => f.FollowerId == domain.FollowerId && f.FollowingId == domain.FollowingId);
}