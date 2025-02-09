namespace GOCAP.Api.Controllers;

[Route("groups")]
public class GroupsController(IGroupService _service,
    IGroupMemberService _groupMemberService,
    IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Get groups by user id with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("{userId}/page")]
    public async Task<QueryResult<GroupModel>> GetByUserIdWithPaging([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetByUserIdWithPagingAsync(userId, queryInfo);
        var result = _mapper.Map<QueryResult<GroupModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get group detail by group id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<GroupDetailModel> GetById([FromRoute] Guid id)
    {
        var group = await _service.GetDetailByIdAsync(id);
        return _mapper.Map<GroupDetailModel>(group);
    }

    /// <summary>
    /// Create a new group.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    public async Task<GroupModel> Create([FromForm] GroupCreationModel model)
    {
        var domain = _mapper.Map<Group>(model);
        var result = await _service.AddAsync(domain);
        return _mapper.Map<GroupModel>(result);
    }

    /// <summary>
    /// Update a group by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] GroupUpdationModel model)
    {
        var domain = _mapper.Map<Group>(model);
        return await _service.UpdateAsync(id, domain);
    }

    /// <summary>
    /// Delete a group by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    /// <summary>
    /// Add or remove a new member to group.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("member")]
    [ValidateModel]
    public async Task<OperationResult> AddOrRemoveMember(GroupMemberCreationModel model)
    {
        var domain = _mapper.Map<GroupMember>(model);
        var result = await _groupMemberService.AddOrRemoveMemberAsync(domain);
        return result;
    }

    [HttpPatch("transfer")]
    [ValidateModel]
    public async Task<OperationResult> TransferGroup(TransferGroupModel model)
    {
        var domain = _mapper.Map<TransferGroup>(model);
        var result = await _service.TransferGroupAsync(domain);
        return result;
    }

    // Join group, leave group, remove member, add member, transfer group..
}
