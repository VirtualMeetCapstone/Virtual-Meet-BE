namespace GOCAP.Services;

[RegisterService(typeof(IGroupMemberService))]
internal class GroupMemberService(
    IGroupMemberRepository _repository,
    IMapper _mapper,
    ILogger<GroupMemberService> _logger
    ) : ServiceBase<GroupMember, GroupMemberEntity>(_repository, _mapper, _logger), IGroupMemberService
{
    private readonly IMapper _mapper = _mapper;
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
            var entity = _mapper.Map<GroupMemberEntity>(domain);
            await _repository.AddAsync(entity);
        }
        return result;
    }
}
