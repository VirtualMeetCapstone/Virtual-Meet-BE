namespace GOCAP.Repository;

[RegisterService(typeof(IStoryViewRepository))]
internal class StoryViewRepository(AppSqlDbContext context) : SqlRepositoryBase<StoryViewEntity>(context), IStoryViewRepository
{
    private readonly AppSqlDbContext _context = context;

    public async Task<bool> CheckViewerExistAsync(Guid storyId, Guid viewerId)
    {
        return await _context.StoryViews
            .AnyAsync(sv => sv.StoryId == storyId && sv.ViewerId == viewerId);
    }

    public async Task<QueryResult<StoryViewDetail>> GetWithPagingAsync(Guid storyId, QueryInfo queryInfo)
    {
        var query = _context.StoryViews
            .Where(sv => sv.StoryId == storyId)
            .Join(_context.Users, sv => sv.ViewerId, u => u.Id, (sv, u) => new StoryViewDetail
            {
                Viewer = new User
                {
                    Id = u.Id,
                    Name = u.Name,
                    Picture = JsonHelper.Deserialize<Media>(u.Picture),
                },
                CreateTime = sv.CreateTime
            });

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderByDescending(sv => sv.CreateTime)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<StoryViewDetail>
        {
            Data = data,
            TotalCount = totalCount
        };
    }

    public async Task<int> DeleteByStoryIdAsync(Guid storyId)
    {
        return await _context.StoryViews
                                .Where(rm => rm.StoryId == storyId)
                                .ExecuteDeleteAsync();
    }
}
