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

    public async Task<OperationResult> TransferGroupAsync(TransferGroup domain)
    {
        // Get group by group id if null then throw exception not found.
        var group = await _repository.GetByIdAsync(domain.GroupId);

        // Check whether the person doing is the group owner.
        if (group.OwnerId != domain.CurrentOwnerId)
        {
            throw new AuthenticationFailedException("Unauthorized access: You do not own this group.");
        }

        // Check whether the new owner is the group member.
        _ = await _groupMemberRepository.GetByGroupMemberAsync(domain.GroupId, domain.NewOwnerId) ?? throw new ResourceNotFoundException($"User {domain.CurrentOwnerId} is not" +
                   $" a member of group {domain.GroupId}");

        group.OwnerId = domain.NewOwnerId;
        var result = await _repository.UpdateAsync(group.Id, group);
        return new OperationResult(result);
    }

    public async Task<GroupDetail> GetDetailByIdAsync(Guid id)
    => await _repository.GetDetailByIdAsync(id);
}
