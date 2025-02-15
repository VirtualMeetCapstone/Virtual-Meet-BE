
namespace GOCAP.Services.Intention;

public interface IStoryService : IServiceBase<Story>
{
    Task<QueryResult<Story>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo);
}
