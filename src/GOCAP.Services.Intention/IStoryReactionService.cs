
namespace GOCAP.Services.Intention;

public interface IStoryReactionService : IServiceBase<StoryReaction>
{
    Task<OperationResult> CreateOrDeleteAsync(StoryReaction domain);
    Task<QueryResult<StoryReaction>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo);
}
