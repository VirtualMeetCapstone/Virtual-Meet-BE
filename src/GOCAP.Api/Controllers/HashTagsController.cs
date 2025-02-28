namespace GOCAP.Api.Controllers;

[Route("hashtags")]
public class HashTagsController(
	IHashTagService _service,
	IRoomHashTagService _roomHashTagService,
	IMapper _mapper) : ApiControllerBase
{
	[HttpGet("search")]
	public async Task<List<HashTagModel>> SearchHashTags([FromQuery] string prefix, [FromQuery] int limit = 20)
	{
		var results = await _service.SearchHashTagsAsync(prefix, limit);
		return _mapper.Map<List<HashTagModel>>(results);
	}
	[HttpGet("rooms/page")]
	public async Task<QueryResult<RoomModel>> GetRoomsByHashTagWithPaging([FromQuery] string tag, [FromQuery] QueryInfo queryInfo)
	{
		var rooms = await _roomHashTagService.GetRoomsByHashTagWithPagingAsync(tag, queryInfo);
		var result = _mapper.Map<QueryResult<RoomModel>>(rooms);
		return result;
	}
}
