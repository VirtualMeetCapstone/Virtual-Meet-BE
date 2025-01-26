namespace GOCAP.Repository;

[RegisterService(typeof(IFollowRepository))]
internal class FollowRepository
    (AppSqlDbContext context, IMapper mapper)
    : SqlRepositoryBase<Follow, UserFollowEntity>(context, mapper), IFollowRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<bool> CheckExistAsync(Guid followerId, Guid followingId)
    {
        return await _context.UserFollows.AnyAsync(f => f.FollowerId == followerId
                                                        && f.FollowingId == followingId);
    }

    public async Task<int> DeleteAsync(Guid followerId, Guid followingId)
    {
        var follow = await _context.UserFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId)
            ?? throw new ResourceNotFoundException("This follow does not exists.");
        _context.UserFollows.Remove(follow);
        return await _context.SaveChangesAsync();
    }
}