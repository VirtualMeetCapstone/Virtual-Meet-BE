namespace GOCAP.Api.Controllers;

[Route("hashtags")]

public class HashtagController(
	IHashtagService _service,
	IMapper _mapper) : ApiControllerBase
{
	[HttpGet("/rooms/hashtags/search")]
	public async Task<List<HashtagModel>> SearchHashtagsAsync([FromQuery] string prefix, [FromQuery] int limit = 20)
	{
		var results = await _service.SearchHashtagsAsync(prefix, limit);
		return _mapper.Map<List<HashtagModel>>(results);
	}
}
