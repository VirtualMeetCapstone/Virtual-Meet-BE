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
    [HttpGet]
    public async Task<QueryResult<RoomModel>> GetWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetWithPagingAsync(queryInfo);
        var result = _mapper.Map<QueryResult<RoomModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<RoomModel> GetById([FromRoute] Guid id)
    {
        var domain = await _service.GetByIdAsync(id);
        return _mapper.Map<RoomModel>(domain);
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
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromForm] RoomUpdationModel model)
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
    [HttpPost("{roomId}/favourite")]
    public async Task<OperationResult> CreateOrDeleteRoomFavourite([FromRoute] Guid roomId, [FromBody] RoomFavouriteCreationModel model)
    {
        var room = _mapper.Map<RoomFavourite>(model);
        room.RoomId = roomId;
        var result = await _roomFavouriteService.CreateOrDeleteAsync(room);
        return result;
    }
	
    [HttpGet("{userId}/favourite")]
	public async Task<QueryResult<RoomFavouriteDetailModel>> GetRoomFavouritesByUserIdWithPaging([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
	{
		var domain = await _roomFavouriteService.GetFavouritesByUserIdWithPagingAsync(userId, queryInfo);
		var result = _mapper.Map<QueryResult<RoomFavouriteDetailModel>>(domain);
		return result;
	}
}
