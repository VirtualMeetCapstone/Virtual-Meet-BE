
namespace GOCAP.Services.Intention;

public interface IStoryService : IServiceBase<Story>
{
    Task<List<Story>> GetActiveStoriesByUserIdAsync(Guid userId);
    Task<QueryResult<Story>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo);
    Task<QueryResult<Story>> GetStoriesByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo);
}
