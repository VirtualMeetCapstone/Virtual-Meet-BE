namespace GOCAP.Repository;

[RegisterService(typeof(IFollowRepository))]
internal class FollowRepository
    (AppSqlDbContext context)
    : SqlRepositoryBase<UserFollowEntity>(context), IFollowRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<UserFollowEntity?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId)
    {
        var entity = await _context.UserFollows.FirstOrDefaultAsync
                                                (f => f.FollowerId == followerId
                                                 && f.FollowingId == followingId);
        return entity;
    }
}