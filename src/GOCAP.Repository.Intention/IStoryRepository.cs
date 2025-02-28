
namespace GOCAP.Repository.Intention;

public interface IStoryRepository : ISqlRepositoryBase<StoryEntity>
{
    Task<QueryResult<StoryEntity>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo);
    Task<QueryResult<StoryEntity>> GetStoriesByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo);
}
