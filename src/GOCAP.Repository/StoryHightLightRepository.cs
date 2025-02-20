
namespace GOCAP.Repository;

[RegisterService(typeof(IStoryHightLightRepository))]
internal class StoryHightLightRepository(
    AppSqlDbContext context
     ) : SqlRepositoryBase<StoryHightLightEntity>(context), IStoryHightLightRepository
{
    private readonly AppSqlDbContext _context = context;

    public async Task<bool> CheckExistAsync(Guid userId, Guid storyId)
    => await _context.StoryHightLights.AnyAsync(h => h.UserId == userId && h.StoryId == storyId);

    public async Task<StoryHightLightEntity?> GetByStoryIdAsync(Guid storyId)
    {
        var result = await _context.StoryHightLights.Where(x => x.StoryId == storyId).ToListAsync();
        if (result.Count > 1)
        {
            throw new InternalException();
        }
        return result.FirstOrDefault();
    }
}