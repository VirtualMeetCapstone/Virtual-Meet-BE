namespace GOCAP.Services.Intention;

public interface IGroupService : IServiceBase<Group>
{
    Task<List<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId);
}
