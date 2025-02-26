namespace GOCAP.Api.Controllers;

[Route("hashtags")]

public class HashtagController(
	IHashtagService _service,
	IRoomHashTagService _roomHashtagService,
	IMapper _mapper) : ApiControllerBase
{
	[HttpGet("search")]
	public async Task<List<HashtagModel>> SearchHashtags([FromQuery] string prefix, [FromQuery] int limit = 20)
	{
		var results = await _service.SearchHashtagsAsync(prefix, limit);
		return _mapper.Map<List<HashtagModel>>(results);
	}
	[HttpGet("/rooms/page")]
	public async Task<QueryResult<RoomModel>> GetRoomByHashTagWithPaging([FromQuery] string tag, [FromQuery] QueryInfo queryInfo)
	{
		var rooms = await _roomHashtagService.GetRoomByHashTagWithPagingAsync(tag, queryInfo);
		var result = _mapper.Map<QueryResult<RoomModel>>(rooms);
		return result;
	}
}
