namespace GOCAP.Repository;

[RegisterService(typeof(IStoryRepository))]
internal class StoryRepository(AppSqlDbContext context, IMapper _mapper) : SqlRepositoryBase<StoryEntity>(context), IStoryRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = _mapper;
    public async Task<QueryResult<StoryEntity>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var followingIds = await _context.UserFollows
                                            .AsNoTracking()
                                            .Where(f => f.FollowerId == userId) // Who this user is following
                                            .Select(f => f.FollowingId) // Get followed people list
                                            .ToListAsync();
        followingIds.Add(userId);
        // Filter story list from who user followed
        var currentTime = DateTimeOffset.UtcNow.Ticks;
        var storiesQuery = _context.Stories
                                        .AsNoTracking()
                                        .Where(s => followingIds.Contains(s.UserId)
                                            && s.CreateTime >= currentTime - TimeSpan.FromHours(24).Ticks
                                            && s.ExpireTime > currentTime)
                                        .OrderByDescending(s => s.CreateTime);

        var stories = await storiesQuery.Skip(queryInfo.Skip)
                                        .Take(queryInfo.Top)
                                        .Include(s => s.User)
                                        .ToListAsync();

        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await storiesQuery.AsNoTracking().CountAsync();
        }


        return new QueryResult<StoryEntity>
        {
            Data = stories,
            TotalCount = totalItems
        };
    }

    public override async Task<StoryEntity> GetByIdAsync(Guid id)
    {
        var entity = await _context.Stories
                                    .AsNoTracking()
                                    .Include(x => x.User)
                                    .FirstOrDefaultAsync(x => x.Id == id);
        return entity ?? throw new ResourceNotFoundException($"Entity with {id} was not found.");
    }

    public async Task<QueryResult<StoryEntity>> GetStoriesByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var currentTime = DateTimeOffset.UtcNow.Ticks;
        var storiesQuery = _context.Stories
                                        .AsNoTracking()
                                        .Where(s => s.UserId == userId)
                                        .OrderByDescending(s => s.CreateTime);

        var stories = await storiesQuery.Skip(queryInfo.Skip)
                                        .Take(queryInfo.Top)
                                        .Include(s => s.User)
                                        .ToListAsync();

        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await storiesQuery.AsNoTracking().CountAsync();
        }


        return new QueryResult<StoryEntity>
        {
            Data = stories,
            TotalCount = totalItems
        };
    }

    public async Task<List<Story>> GetActiveStoriesByUserIdAsync(Guid userId)
    {
        var currentTime = DateTimeOffset.UtcNow.Ticks;
        var stories = await _context.Stories
                                        .AsNoTracking()
                                        .Where(s => s.UserId == userId
                                            && s.CreateTime >= currentTime - TimeSpan.FromHours(24).Ticks
                                            && s.ExpireTime > currentTime)
                                        .OrderByDescending(s => s.CreateTime)
                                        .ToListAsync();
        return _mapper.Map<List<Story>>(stories);
    }

    public async Task<Guid> GetUserIdByStoryIdAsync(Guid storyId)
    => await _context.Stories.AsNoTracking()
                             .Where(s => s.Id == storyId)
                             .Select(s => s.UserId)
                             .FirstOrDefaultAsync();
}
