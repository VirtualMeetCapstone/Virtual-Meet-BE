namespace GOCAP.Services.Intention;

public interface IGroupService : IServiceBase<Group>
{
    Task<QueryResult<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId);
    Task<OperationResult> TransferGroupAsync(TransferGroup domain);
    Task<GroupDetail> GetDetailByIdAsync(Guid id);
}
