namespace GOCAP.Api.Controllers;

[Route("stories")]
public class StoriesController(
    IStoryService _service,
    IStoryReactionService _storyReactionService,
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

    /// <summary>
    /// Get the stories which user is following.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("{userId}/friends")]
    public async Task<QueryResult<StoryModel>> GetFollowingStoriesWithPaging([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var stories = await _service.GetFollowingStoriesWithPagingAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<StoryModel>>(stories);
    }

    /// <summary>
    /// Get story detail by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<StoryDetailModel> GetById([FromRoute] Guid id)
    {
        var domain = await _service.GetByIdAsync(id);
        return _mapper.Map<StoryDetailModel>(domain);
    }

    /// <summary>
    /// Delete story by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    /// <summary>
    /// Create or delete story reaction.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{storyId}/reactions")]
    public async Task<OperationResult> CreateOrDeleteReaction([FromRoute] Guid storyId, [FromBody] StoryReactionCreationModel model)
    {
        var domain = _mapper.Map<StoryReaction>(model);
        domain.StoryId = storyId;
        var result = await _storyReactionService.CreateOrDeleteAsync(domain);
        return result;
    }

    [HttpGet("{storyId}/reactions/page")]
    public async Task<QueryResult<StoryReactionDetailModel>> GetReactionDetailsWithPaging([FromRoute] Guid storyId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _storyReactionService.GetReactionDetailsWithPagingAsync(storyId, queryInfo);
        var result = _mapper.Map<QueryResult<StoryReactionDetailModel>>(domain);
        return result;
    }
}
