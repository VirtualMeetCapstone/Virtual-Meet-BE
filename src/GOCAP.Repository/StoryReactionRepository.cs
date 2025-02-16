
namespace GOCAP.Repository;

[RegisterService(typeof(IStoryReactionRepository))]
internal class StoryReactionRepository(
    AppSqlDbContext context,
     IMapper mapper
     ) : SqlRepositoryBase<StoryReaction, StoryReactionEntity>(context, mapper), IStoryReactionRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<StoryReaction?> GetByStoryAndUserAsync(Guid storyId, Guid userId)
    {
        var entity = await _context.StoryReactions.FirstOrDefaultAsync
                                                (sr => sr.StoryId == storyId
                                                 && sr.UserId == userId);
        return _mapper.Map<StoryReaction?>(entity);
    }

    public async Task<QueryResult<StoryReaction>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo)
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

        return new QueryResult<StoryReaction>
        {
            Data = _mapper.Map<List<StoryReaction>>(reactions),
            TotalCount = totalItems
        };

    }
}