namespace GOCAP.Api.Controllers;

public class StoriesController(
    IStoryService _service,
    IStoryReactionService _storyReactionService,
    IStoryViewService _storyViewService,
    IStoryHighlightService _storyHightLightService,
    IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Upload a new story.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>story model</returns>
    [HttpPost("stories")]
    [ValidateModel]
    public async Task<StoryModel> CreateNewStory([FromForm] StoryCreationModel model)
    {
        var domain = _mapper.Map<Story>(model);
        var result = await _service.AddAsync(domain);
        return _mapper.Map<StoryModel>(result);
    } 

    /// <summary>
    /// Get story detail by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>StoryDetailModel</returns>
    [HttpGet("stories/{id}")]
    public async Task<StoryDetailModel> GetStoryById([FromRoute] Guid id)
    {
        var domain = await _service.GetByIdAsync(id);
        return _mapper.Map<StoryDetailModel>(domain);
    }

    /// <summary>
    /// Delete story by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>OperationResult</returns>
    [HttpDelete("stories/{id}")]
    public async Task<OperationResult> DeleteStory([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    /// <summary>
    /// Remove hight light story.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="storyHighlightId"></param>
    /// <returns>Operation result</returns>
    [HttpDelete("stories/{storyId}/highlight/{storyHighlightId}")]
    public async Task<OperationResult> RemoveHightlightStory([FromRoute] Guid storyId, [FromRoute] Guid storyHighlightId)
    => await _storyHightLightService.DeleteAsync(storyId, storyHighlightId);

    /// <summary>
    /// Create or delete story reaction.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="model"></param>
    /// <returns>OperationResult</returns>
    [HttpPost("stories/{storyId}/reactions")]
    public async Task<OperationResult> CreateOrDeleteReaction([FromRoute] Guid storyId, [FromBody] StoryReactionCreationModel model)
    {
        var domain = _mapper.Map<StoryReaction>(model);
        domain.StoryId = storyId;
        var result = await _storyReactionService.CreateOrDeleteAsync(domain);
        return result;
    }

    /// <summary>
    /// Get reaction details with paging by story id.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="queryInfo"></param>
    /// <returns>StoryReactionDetailModel</returns>
    [HttpGet("stories/{storyId}/reactions")]
    public async Task<QueryResult<StoryReactionDetailModel>> GetReactionDetailsWithPaging([FromRoute] Guid storyId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _storyReactionService.GetReactionDetailsWithPagingAsync(storyId, queryInfo);
        var result = _mapper.Map<QueryResult<StoryReactionDetailModel>>(domain);
        return result;
    }

    /// <summary>
    /// Record when one user views the story.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="model"></param>
    /// <returns>StoryViewModel</returns>
    [HttpPost("stories/{storyId}/views")]
    public async Task<StoryViewModel> RecordStoryView([FromRoute] Guid storyId, [FromBody] StoryViewCreationModel model)
    {
        var domain = _mapper.Map<StoryView>(model);
        domain.StoryId = storyId;
        var result = await _storyViewService.AddAsync(domain);
        return _mapper.Map<StoryViewModel>(result);
    }

    /// <summary>
    /// Get story viewer with paging by story id.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("stories/{storyId}/views")]
    public async Task<QueryResult<StoryViewDetailModel>> GetStoryViewers([FromRoute] Guid storyId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _storyViewService.GetWithPagingAsync(storyId, queryInfo);
        var result = _mapper.Map<QueryResult<StoryViewDetailModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get stories by user id.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("users/{userId}/stories")]
    public async Task<QueryResult<StoryUserModel>> GetStoriesByUserId([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetStoriesByUserIdWithPagingAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<StoryUserModel>>(domain);
    }

    /// <summary>
    /// Get the stories which user is following.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="queryInfo"></param>
    /// <returns>QueryResult<StoryModel></returns>
    [HttpGet("users/{userId}/followings/stories")]
    public async Task<QueryResult<StoryModel>> GetFollowingStoriesWithPaging([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var stories = await _service.GetFollowingStoriesWithPagingAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<StoryModel>>(stories);
    }

    /// <summary>
    /// Add one hight light story to user profile.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="model"></param>
    /// <returns>StoryHightLightModel</returns>
    [HttpPost("users/{userId}/stories/{storyId}/highlight")]
    public async Task<StoryHighlightModel> AddHighlightStory([FromRoute] Guid userId, [FromRoute] Guid storyId, [FromBody] StoryHighlightCreationModel model)
    {
        var domain = _mapper.Map<StoryHighlight>(model);
        domain.StoryId = storyId;
        domain.UserId = userId;
        var result = await _storyHightLightService.AddAsync(domain);
        return _mapper.Map<StoryHighlightModel>(result);
    }

    [HttpGet("users/{userId}/story-highlights")]
    public async Task<List<List<StoryModel>>> GetStoryHighlightByUserId([FromRoute] Guid userId)
    {
        var result = await _storyHightLightService.GetStoryHighlightByUserIdAsync(userId);
        return _mapper.Map<List<List<StoryModel>>>(result);
    }
}
