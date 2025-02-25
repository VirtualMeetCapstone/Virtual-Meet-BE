namespace GOCAP.Api.Controllers;

[Route("roomhashtags")]
public class RoomHashtagsController(
	IRoomHashtagService _service,
	IMapper _mapper) : ApiControllerBase
{
	[HttpGet("/rooms/hashtags/page")]
	public async Task<QueryResult<RoomModel>> GetRoomHashTagWithPaging([FromQuery] string tag, [FromQuery] QueryInfo queryInfo)
	{
		var rooms = await _service.GetRoomHashTagWithPagingAsync(tag, queryInfo);
		var result = _mapper.Map<QueryResult<RoomModel>>(rooms);
		return result;
	}
}
