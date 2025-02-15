
namespace GOCAP.Repository.Intention;

public interface IStoryRepository : ISqlRepositoryBase<Story>
{
    Task<QueryResult<Story>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo);
}
