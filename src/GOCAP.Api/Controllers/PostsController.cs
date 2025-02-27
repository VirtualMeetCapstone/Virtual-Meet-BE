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
        var domain = await _service.GetWithPagingAsync(queryInfo);
        var result = _mapper.Map<QueryResult<PostModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get list post by postID with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("{postId}")]
    public async Task<ActionResult<PostModel>> GetById([FromRoute] Guid postId)
    {
        var post = await _service.GetDetailByIdAsync(postId);
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

    [HttpDelete("{postId}")]
    public async Task<OperationResult> Delete([FromRoute] Guid postId)
    {
        return await _service.DeleteByIdAsync(postId);
    }

    [HttpPost("react")]
    public async Task<OperationResult> ReactOrUnReact([FromBody] PostReactionCreationModel model)
    {
        var domain = _mapper.Map<PostReaction>(model);
        var result = await _postReactionService.ReactOrUnreactedAsync(domain);
        return result;
    }

    /// <summary>
    /// Get list user-reaction of post by with paging.
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

    /// <summary>
    /// Get list post by UserID with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("user/{userId}")]
    public async Task<QueryResult<PostModel>> GetPostByUserIdAsync([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetPostByUserIdAsync(userId, queryInfo);
        var result = _mapper.Map<QueryResult<PostModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get list post by UserID have reacted with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("user/reacted/{userId}")]
    public async Task<QueryResult<PostModel>> GetPostsUserReactedAsync([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetPostsUserReactedAsync(userId, queryInfo);
        var result = _mapper.Map<QueryResult<PostModel>>(domain);
        return result;
    }

}