namespace GOCAP.Api.Controllers;

[Route("stories")]
public class StoriesController(
    IStoryService _service,
    IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Upload a new story.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>story model</returns>
    [HttpPost]
    [ValidateModel]
    public async Task<StoryModel> Create([FromForm] StoryCreationModel model)
    {
        var domain = _mapper.Map<Story>(model);
        var result = await _service.AddAsync(domain);
        return _mapper.Map<StoryModel>(result);
    }

    [HttpGet("{userId}/friends")]
    public async Task<QueryResult<StoryModel>> GetFollowingStoriesWithPaging([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var stories = await _service.GetFollowingStoriesWithPagingAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<StoryModel>>(stories);
    }
}
