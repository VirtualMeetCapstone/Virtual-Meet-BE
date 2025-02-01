namespace GOCAP.Repository.Intention;

public interface IGroupRepository : ISqlRepositoryBase<Group>
{
    Task<List<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId);
}
