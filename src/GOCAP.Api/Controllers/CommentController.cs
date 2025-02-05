namespace GOCAP.Api.Controllers;

[Route("comments")]
public class CommentController(ICommentService _commentService, IMapper _mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Comment>> GetAll()
    {
        var domain = await _commentService.GetAllAsync();
        return _mapper.Map<List<Comment>>(domain);
    }

    [HttpPost]
    public async Task<Comment> Create([FromBody] Comment model)
    {
        var domain = _mapper.Map<Comment>(model);
        return await _commentService.AddAsync(domain);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _commentService.DeleteByIdAsync(id);
    }
}
