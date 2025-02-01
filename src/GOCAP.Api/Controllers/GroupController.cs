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
    [HttpGet("page")]
    public async Task<QueryResult<List<GroupModel>>> GetByUserIdWithPaging([FromQuery] QueryInfo queryInfo, [FromRoute] Guid userId)
    {
        var domain = await _service.GetByUserIdWithPagingAsync(queryInfo, userId);
        var result = _mapper.Map<QueryResult<List<GroupModel>>>(domain);
        return result;
    }

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
    [HttpPatch]
    public async Task<OperationResult> Update(Guid id, [FromBody] GroupCreationModel model)
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

    // Join group, leave group
}
