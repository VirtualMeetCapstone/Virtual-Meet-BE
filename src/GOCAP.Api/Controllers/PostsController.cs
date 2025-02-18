namespace GOCAP.Api.Controllers;

[Route("posts")]
public class PostsController(
    IPostService _service,
    IMapper _mapper) : ApiControllerBase
{

    [HttpGet("{id}")]
    public async Task<PostModel?> GetById([FromRoute] Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return _mapper.Map<PostModel>(result);
    }

    [HttpPost]
    public async Task<PostModel> Create([FromForm] PostCreationModel model)
    {
        return await Task.FromResult(new PostModel());
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }

    [HttpPost("like")]
    public async Task<OperationResult> LikeOrUnlike([FromBody] PostReactionCreationModel model)
    {
        var domain = _mapper.Map<PostReaction>(model);
        var result = await _service.ReactOrUnreactAsync(domain);
        return result;
    }
}
