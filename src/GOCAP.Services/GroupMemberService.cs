namespace GOCAP.Services;

[RegisterService(typeof(IGroupMemberService))]
internal class GroupMemberService(
    IGroupMemberRepository _repository,
    ILogger<GroupMemberService> _logger
    ) : ServiceBase<GroupMember>(_repository, _logger), IGroupMemberService
{

    /// <summary>
    /// Add or remove a group member.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public async Task<OperationResult> AddOrRemoveMemberAsync(GroupMember domain)
    {
        var result = new OperationResult(true);
        var groupMember = await _repository.GetByGroupMemberAsync(domain.GroupId, domain.UserId);
        if (groupMember != null)
        {
            _logger.LogInformation("Start deleting an entity of type {EntityType}.", typeof(GroupMember).Name);
            var resultDelete = await _repository.DeleteByIdAsync(groupMember.Id);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(GroupMember).Name);
            domain.InitCreation();
            await _repository.AddAsync(domain);
        }
        return result;
    }
}
