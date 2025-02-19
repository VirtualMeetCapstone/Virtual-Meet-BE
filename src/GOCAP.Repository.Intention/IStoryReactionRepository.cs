namespace GOCAP.Repository.Intention;

public interface IStoryReactionRepository : ISqlRepositoryBase<StoryReactionEntity>
{
    Task<StoryReactionEntity?> GetByStoryAndUserAsync(Guid storyId, Guid userId);
    Task<QueryResult<StoryReactionEntity>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo);
}
