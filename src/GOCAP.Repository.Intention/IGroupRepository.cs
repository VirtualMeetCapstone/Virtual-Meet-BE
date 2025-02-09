namespace GOCAP.Repository.Intention;

public interface IGroupRepository : ISqlRepositoryBase<Group>
{
    Task<GroupCount> GetGroupCountsAsync();
    Task<QueryResult<Group>> GetByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo);
    Task<GroupDetail> GetDetailByIdAsync(Guid id);
}
