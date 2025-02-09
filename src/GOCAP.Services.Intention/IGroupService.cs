namespace GOCAP.Services.Intention;

public interface IGroupService : IServiceBase<Group>
{
    Task<GroupCount> GetGroupCountsAsync();
    Task<QueryResult<Group>> GetByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo);
    Task<OperationResult> TransferGroupAsync(TransferGroup domain);
    Task<GroupDetail> GetDetailByIdAsync(Guid id);
}
