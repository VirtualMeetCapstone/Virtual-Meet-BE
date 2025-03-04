namespace GOCAP.Repository.Intention;

public interface IStoryViewRepository : ISqlRepositoryBase<StoryViewEntity>
{
    Task<QueryResult<StoryViewDetail>> GetWithPagingAsync(Guid storyId, QueryInfo queryInfo);
    Task<bool> CheckViewerExistAsync(Guid storyId, Guid viewerId);
    Task<int> DeleteByStoryIdAsync(Guid storyId);
}
