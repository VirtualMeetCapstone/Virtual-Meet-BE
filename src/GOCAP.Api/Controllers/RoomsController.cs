namespace GOCAP.Api.Controllers;

[Route("rooms")]
public class RoomsController(IRoomService _service,
    IRoomFavouriteService _roomFavouriteService,
    IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Get rooms with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<QueryResult<RoomModel>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetByPageAsync(queryInfo);
        var result = _mapper.Map<QueryResult<RoomModel>>(domain);
        return result;
    }

    /// <summary>
    /// Create a new room.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<RoomModel> Create([FromForm] RoomCreationModel model)
    {
        var room = _mapper.Map<Room>(model);
        var result = await _service.AddAsync(room);
        return _mapper.Map<RoomModel>(result);
    }

    /// <summary>
    /// Update a room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromForm] RoomCreationModel model)
    {
        var domain = _mapper.Map<Room>(model);
        domain.Id = id;
        return await _service.UpdateAsync(id, domain);
    }

    /// <summary>
    /// Delete a room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    /// <summary>
    /// Create or delete a room favourite.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("favourite")]
    public async Task<OperationResult> CreateOrDeleteRoomFavourite([FromBody] RoomFavouriteCreationModel model)
    {
        var room = _mapper.Map<RoomFavourite>(model);
        var result = await _roomFavouriteService.CreateOrDeleteAsync(room);
        return result;
    }
}
