namespace GOCAP.Repository.Intention;

public interface IStoryRepository : ISqlRepositoryBase<StoryEntity>
{
    Task<List<Story>> GetActiveStoriesByUserIdAsync(Guid userId);
    Task<QueryResult<StoryEntity>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo);
    Task<QueryResult<StoryEntity>> GetStoriesByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo);
}
