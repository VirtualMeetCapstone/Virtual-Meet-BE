namespace GOCAP.Repository.Intention;

public interface IStoryViewRepository : ISqlRepositoryBase<StoryViewEntity>
{
    Task<QueryResult<StoryViewDetail>> GetWithPagingAsync(Guid storyId, QueryInfo queryInfo);
}
