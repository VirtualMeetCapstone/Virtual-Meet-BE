
namespace GOCAP.Repository.Intention;

public interface IStoryHighlightRepository : ISqlRepositoryBase<StoryHightLightEntity>
{
    Task<StoryHightLightEntity?> GetByStoryIdAsync(Guid storyId, bool isAsNoTracking = false);
    Task<bool> CheckExistAsync(Guid userId, Guid storyId);
    Task<int> DeleteAsync(Guid storyId, Guid storyHighlightId);
    Task<List<List<Story>>> GetStoryHighlightByUserIdAsync(Guid userId);
    Task<int> DeleteByStoryIdAsync(Guid storyId);
}
