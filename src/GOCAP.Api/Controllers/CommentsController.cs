namespace GOCAP.Api.Controllers;

[Route("posts/{postId}/comments")]
public class CommentsController(ICommentService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<QueryResult<CommentModel>> GetByPostIdWithPaging([FromRoute] Guid postId,
        [FromQuery] QueryInfo queryInfo)
    {
        var comments = await _service.GetByPostIdWithPagingAsync(postId, queryInfo);
        var result = _mapper.Map<QueryResult<CommentModel>>(comments);
        return result;
    }

    [HttpPost]
    [ValidateModel]
    public async Task<CommentModel> Create([FromRoute] Guid postId, [FromBody] CommentCreationModel model)
    {
        var domain = _mapper.Map<Comment>(model);
        domain.PostId = postId;
        var result = await _service.AddAsync(domain);
        return _mapper.Map<CommentModel>(result);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    [HttpGet("replies/{commentId}")]
    public async Task<QueryResult<CommentModel>> GetReplies([FromRoute] Guid commentId, [FromQuery] QueryInfo queryInfo)
    {
        //use replyModel
        var replies = await _service.GetRepliesAsyncWithPagingAsync(commentId, queryInfo);
        var result = _mapper.Map<QueryResult<CommentModel>>(replies);
        return result;
    }

    [HttpPost("react")]
    public async Task<IActionResult> ReactOrUnreacted([FromBody] CommentReactionModel reaction)
    {
        var domain = _mapper.Map<CommentReaction>(reaction);
        var result = await _service.ReactOrUnreactedAsync(domain);
       
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
