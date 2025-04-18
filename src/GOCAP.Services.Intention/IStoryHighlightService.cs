
namespace GOCAP.Services.Intention;

public interface IStoryHighlightService : IServiceBase<StoryHighlight>
{
    Task<OperationResult> DeleteAsync(Guid storyId, Guid storyHighlightId);
    Task<List<List<Story>>> GetStoryHighlightByUserIdAsync(Guid userId);
}
