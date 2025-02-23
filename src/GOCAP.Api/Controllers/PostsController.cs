namespace GOCAP.Api.Controllers;

[Route("posts")]
public class PostsController(
    IPostService _service,
       IPostReactionService _postReactionService,
    IMapper _mapper) : ApiControllerBase
{

    /// <summary>
    /// Get posts by with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<QueryResult<PostModel>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetByPageAsync(queryInfo);
        var result = _mapper.Map<QueryResult<PostModel>>(domain);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostModel>> GetById([FromRoute] Guid id)
    {
        var post = await _service.GetDetailByIdAsync(id);
        var postModel = _mapper.Map<PostModel>(post);
        return Ok(postModel);
    }

    [HttpPost]
    public async Task<PostModel> Create([FromForm] PostCreationModel model)
    {
        var post = _mapper.Map<Post>(model);
        var result = await _service.AddAsync(post);
        return _mapper.Map<PostModel>(result);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    [HttpPost("react")]
    public async Task<OperationResult> ReactOrUnReact([FromBody] PostReactionCreationModel model)
    {
        var domain = _mapper.Map<PostReaction>(model);
        var result = await _postReactionService.ReactOrUnreactedAsync(domain);
        return result;
    }

    /// <summary>
    /// Get list user-reaction by with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("users-reaction/{postId}")]
    [AllowAnonymous]
    public async Task<QueryResult<UserReactionPostModel>> GetUserReactionsByPostIdAsync([FromRoute] Guid postId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _postReactionService.GetUserReactionsByPostIdAsync(postId, queryInfo);
        var result = _mapper.Map<QueryResult<UserReactionPostModel>>(domain);
        return result;
    }

}

