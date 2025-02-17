namespace GOCAP.Repository.Intention;

public interface IGroupRepository : ISqlRepositoryBase<GroupEntity>
{
    Task<GroupCount> GetGroupCountsAsync();
    Task<QueryResult<GroupEntity>> GetByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo);
    Task<GroupDetail> GetDetailByIdAsync(Guid id);
}
