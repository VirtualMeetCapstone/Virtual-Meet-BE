namespace GOCAP.Api.Controllers;

[Route("groups")]
[ApiController]
public class GroupController(IGroupService _service, IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Get groups by user id with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("{userId}/page")]
    public async Task<QueryResult<GroupModel>> GetByUserIdWithPaging([FromQuery] QueryInfo queryInfo, [FromRoute] Guid userId)
    {
        var domain = await _service.GetByUserIdWithPagingAsync(queryInfo, userId);
        var result = _mapper.Map<QueryResult<GroupModel>>(domain);
        return result;
    }

    /// Get group detail by group id.

    /// <summary>
    /// Create a new group.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<GroupModel> Create([FromBody] GroupCreationModel model)
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
        domain.Id = id; 
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

    // Join group, leave group, remove member, add member, transfer group..
}
