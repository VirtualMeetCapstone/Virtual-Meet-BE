namespace GOCAP.Api.Controllers;

[Route("posts")]
public class PostsController(
    IPostService _service,
    IMapper _mapper) : ApiControllerBase
{

    [HttpGet("{id}")]
    public async Task<PostModel?> GetById([FromRoute] Guid id)
    {
        var result = await _service.GetByIdAsync(id) ?? throw new ResourceNotFoundException("The post does not exist");
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
    public async Task<OperationResult> LikeOrUnlike([FromBody] PostLikeCreationModel model)
    {
        var domain = _mapper.Map<PostLike>(model);
        var result = await _service.LikeOrUnlikeAsync(domain);
        return result;
    }
}
