namespace GOCAP.Services;

[RegisterService(typeof(IGroupService))]
internal class GroupService(
    IGroupRepository _repository,
    IGroupMemberRepository _groupMemberRepository,
    IUnitOfWork _unitOfWork,
    ILogger<GroupService> _logger
    ) : ServiceBase<Group>(_repository, _logger), IGroupService
{
    public async Task<QueryResult<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId)
    {
        return await _repository.GetByUserIdWithPagingAsync(queryInfo, userId);
    }

    public override async Task<Group> AddAsync(Group group)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Group).Name);
        group.InitCreation();
        return await _repository.AddAsync(group);
    }

    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Group).Name);
        try
        {
            // Begin transaction by unit of work to make sure the consistency
            await _unitOfWork.BeginTransactionAsync();

            await _groupMemberRepository.DeleteByGroupIdAsync(id);
            var result = await _repository.DeleteByIdAsync(id);

            // Commit if success
            await _unitOfWork.CommitTransactionAsync();
            return new OperationResult(result > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(Group).Name);
            // Rollback if fail
            await _unitOfWork.RollbackTransactionAsync();
            return new OperationResult(false);
        }
    }

    public async Task<GroupCount> GetGroupCountsAsync()
    {
        return await _repository.GetGroupCountsAsync();
    }
}
