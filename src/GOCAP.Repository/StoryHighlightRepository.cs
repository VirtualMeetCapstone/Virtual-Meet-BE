namespace GOCAP.Repository;

[RegisterService(typeof(IStoryHighlightRepository))]
internal class StoryHighlightRepository(
    AppSqlDbContext context,
    IMapper _mapper
     ) : SqlRepositoryBase<StoryHightLightEntity>(context), IStoryHighlightRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = _mapper;

    public async Task<bool> CheckExistAsync(Guid userId, Guid storyId)
    => await _context.StoryHightLights.AnyAsync(h => h.UserId == userId && h.StoryId == storyId);

    public async Task<int> DeleteAsync(Guid storyId, Guid storyHighlightId)
    {
        var storyHightlightEntity = await _context.StoryHightLights
            .Where(d => d.Id == storyHighlightId && d.StoryId == storyId)
            .Select(d => new StoryHightLightEntity
            {
                Id = d.Id,
                PrevStoryId = d.PrevStoryId,
                NextStoryId = d.NextStoryId
            })
            .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException($"Story hight light {storyHighlightId} was not found.");
        var prevStoryId = storyHightlightEntity.PrevStoryId;
        var nextStoryId = storyHightlightEntity.NextStoryId;

        if (prevStoryId != null)
        {
            var prevStory = await _context.StoryHightLights.FirstOrDefaultAsync(x => x.Id == prevStoryId)
                ?? throw new InternalException();
            prevStory.NextStoryId = nextStoryId;
            _context.Entry(prevStory).State = EntityState.Modified;
        }

        if (nextStoryId != null)
        {
            var nextStory = await _context.StoryHightLights.FirstOrDefaultAsync(x => x.Id == nextStoryId)
                ?? throw new InternalException();
            nextStory.PrevStoryId = prevStoryId;
            _context.Entry(nextStory).State = EntityState.Modified;
        }

        storyHightlightEntity.PrevStory = null;
        storyHightlightEntity.NextStory = null;
        _context.Entry(storyHightlightEntity).State = EntityState.Modified;

        _context.StoryHightLights.Remove(storyHightlightEntity);
        return await _context.SaveChangesAsync();
    }

    public async Task<StoryHightLightEntity?> GetByStoryIdAsync(Guid storyId)
    {
        var result = await _context.StoryHightLights.Where(x => x.StoryId == storyId).ToListAsync();
        if (result.Count > 1)
        {
            throw new InternalException();
        }
        return result.FirstOrDefault();
    }

    public async Task<List<List<Story>>> GetStoryHighlightByUserIdAsync(Guid userId)
    {
        // Get all stories by user id 
        var storyHighlights = await _context.StoryHightLights
                                         .AsNoTracking()
                                         .Where(h => h.UserId == userId)
                                         .ToListAsync();

        if (storyHighlights == null || storyHighlights.Count == 0)
        {
            return [];
        }

        var startingStories = storyHighlights.Where(s => s.PrevStoryId == null).ToList();
        var storyGroups = new List<List<Guid>>();

        foreach (var startHighlight in startingStories)
        {
            var group = new List<Guid> { startHighlight.StoryId };
            var currentHighlight = startHighlight;

            while (currentHighlight.NextStoryId != null)
            {
                currentHighlight = storyHighlights.FirstOrDefault(s => s.Id == currentHighlight.NextStoryId);
                if (currentHighlight != null)
                    group.Add(currentHighlight.StoryId);
                else
                    break;
            }

            storyGroups.Add(group);
        }

        var allStoryIds = storyGroups.SelectMany(g => g).Distinct().ToList();
        var storiesDict = await _context.Stories.Where(s => allStoryIds.Contains(s.Id))
                                                .ToDictionaryAsync(s => s.Id);

        var result = storyGroups
                        .Select(group => group
                            .Where(id => storiesDict.ContainsKey(id))
                            .Select(id => storiesDict[id])
                            .ToList()
                        ).ToList();
        return _mapper.Map<List<List<Story>>>(result);
    }
}