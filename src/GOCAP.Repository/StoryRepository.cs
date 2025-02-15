
namespace GOCAP.Repository;

[RegisterService(typeof(IStoryRepository))]
internal class StoryRepository(AppSqlDbContext context,
     IMapper mapper) : SqlRepositoryBase<Story, StoryEntity>(context, mapper), IStoryRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<QueryResult<Story>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var followingIds = await _context.UserFollows
                                            .Where(f => f.FollowerId == userId) // Who this user is following
                                            .Select(f => f.FollowingId) // Get followed people list
                                            .ToListAsync();

        // Filter story list from who user followed
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var storiesQuery = _context.Stories
                                        .Where(s => followingIds.Contains(s.UserId)
                                            && s.CreateTime >= currentTime - 86400
                                            && s.ExpireTime > currentTime)
                                        .OrderByDescending(s => s.CreateTime);

        var stories = await storiesQuery.Skip(queryInfo.Skip)
                                        .Take(queryInfo.Top)
                                        .Include(s => s.User)
                                        .ToListAsync();

        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await storiesQuery.CountAsync();
        }


        return new QueryResult<Story>
        {
            Data = _mapper.Map<List<Story>>(stories),
            TotalCount = totalItems
        };
    }
}
