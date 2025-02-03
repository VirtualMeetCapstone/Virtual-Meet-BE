namespace GOCAP.Repository.Intention;

public interface IGroupRepository : ISqlRepositoryBase<Group>
{
    Task<QueryResult<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId);
}
