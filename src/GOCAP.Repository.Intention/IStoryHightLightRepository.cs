
namespace GOCAP.Repository.Intention;

public interface IStoryHightLightRepository : ISqlRepositoryBase<StoryHightLightEntity>
{
    Task<StoryHightLightEntity?> GetByStoryIdAsync(Guid storyId);
    Task<bool> CheckExistAsync(Guid userId, Guid storyId);
    Task<int> DeleteAsync(Guid storyId, Guid storyHighlightId);
    Task<List<List<Story>>> GetStoryHighlightByUserIdAsync(Guid userId);
}
