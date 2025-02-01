
namespace GOCAP.Services;

[RegisterService(typeof(IGroupService))]
internal class GroupService(
    IGroupRepository _repository,
    ILogger<GroupService> _logger
    ) : ServiceBase<Group>(_repository, _logger), IGroupService
{
    public async Task<List<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId)
    {
        return await _repository.GetByUserIdWithPagingAsync(queryInfo, userId);
    }
}
