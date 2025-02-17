
namespace GOCAP.Repository;

[RegisterService(typeof(IStoryReactionRepository))]
internal class StoryReactionRepository(
    AppSqlDbContext context
     ) : SqlRepositoryBase<StoryReactionEntity>(context), IStoryReactionRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<StoryReactionEntity?> GetByStoryAndUserAsync(Guid storyId, Guid userId)
    {
        var entity = await _context.StoryReactions.FirstOrDefaultAsync
                                                (sr => sr.StoryId == storyId
                                                 && sr.UserId == userId);
        return entity;
    }

    public async Task<QueryResult<StoryReactionEntity>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo)
    {
        var reactions = await _context.StoryReactions
                                            .Include(sr => sr.User)
                                            .Where(r => r.StoryId == storyId)
                                            .ToListAsync();
        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await _context.StoryReactions.CountAsync();
        }

        return new QueryResult<StoryReactionEntity>
        {
            Data = reactions,
            TotalCount = totalItems
        };

    }
}