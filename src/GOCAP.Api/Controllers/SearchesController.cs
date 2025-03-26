namespace GOCAP.Api.Controllers;

[Route("searches")]
public class SearchesController (ISearchHistoryService _service, IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Save new search history.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>SearchHistoryModel</returns>
    [HttpPost("history/save")]
    [ValidateModel]
    public async Task<SearchHistoryModel> SaveSearchHistory([FromBody] SearchHistoryCreationModel model)
    {
        var domain = _mapper.Map<SearchHistory>(model);
        var result = await _service.AddAsync(domain);
        return _mapper.Map<SearchHistoryModel>(result);
    }

    /// <summary>
    /// Get popular search suggestions.
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="limit"></param>
    /// <returns>list string</returns>
    [HttpGet("suggestions")]
    public async Task<List<string>> GetPopularSearchSuggestions([FromQuery] string prefix, [FromQuery]
    int limit)
    {
        if (limit == 0)
        {
            limit = 30;
        }
        return await _service.GetPopularSearchSuggestionsAsync(prefix, limit);
    }

    [HttpGet("{userId}")]
    public async Task<List<string>> GetSearchByUserId([FromRoute] Guid userId, [FromQuery]
    int limit)
    {
        if (limit == 0)
        {
            limit = 20;
        }
        return await _service.GetSearchByUserIdAsync(userId, limit);
    }
}
