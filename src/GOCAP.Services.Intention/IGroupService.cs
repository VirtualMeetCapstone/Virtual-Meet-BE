namespace GOCAP.Services.Intention;

public interface IGroupService : IServiceBase<Group>
{
    Task<QueryResult<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId);
}
