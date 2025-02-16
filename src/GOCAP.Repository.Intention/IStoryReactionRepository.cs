

namespace GOCAP.Repository.Intention;

public interface IStoryReactionRepository : ISqlRepositoryBase<StoryReaction>
{
    Task<StoryReaction?> GetByStoryAndUserAsync(Guid storyId, Guid userId);
    Task<QueryResult<StoryReaction>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo);
}
