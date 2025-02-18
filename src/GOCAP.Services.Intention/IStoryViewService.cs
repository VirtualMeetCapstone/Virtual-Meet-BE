
namespace GOCAP.Services.Intention;

public interface IStoryViewService : IServiceBase<StoryView>
{
    Task<QueryResult<StoryViewDetail>> GetWithPagingAsync(Guid storyId, QueryInfo queryInfo);
}
