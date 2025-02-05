namespace GOCAP.Repository;

[RegisterService(typeof(IFollowRepository))]
internal class FollowRepository
    (AppSqlDbContext context, IMapper mapper)
    : SqlRepositoryBase<Follow, UserFollowEntity>(context, mapper), IFollowRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<Follow?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId)
    {
        var entity = await _context.UserFollows.FirstOrDefaultAsync
                                                (f => f.FollowerId == followerId
                                                 && f.FollowingId == followingId);
        return _mapper.Map<Follow?>(entity);
    }
}