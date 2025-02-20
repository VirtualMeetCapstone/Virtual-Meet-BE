
namespace GOCAP.Services.Intention;

public interface IStoryHightLightService : IServiceBase<StoryHightLight>
{
    Task<OperationResult> DeleteAsync(Guid storyId, Guid storyHighlightId);
    Task<List<List<Story>>> GetStoryHighlightByUserIdAsync(Guid userId);
}
