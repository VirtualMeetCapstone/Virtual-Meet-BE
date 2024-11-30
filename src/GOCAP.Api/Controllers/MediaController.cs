namespace GOCAP.Api.Controllers;

[Route("medias")]
[ApiController]
public class MediaController(IMediaService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Media>> GetAll()
    {
        var domain = await _service.GetAllAsync();
        return _mapper.Map<List<Media>>(domain);
    }

    [HttpGet("page")]
    public async Task<QueryResult<Media>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var result = await _service.GetByPageAsync(queryInfo);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<Media?> GetById([FromRoute] Guid id)
    {
        var domain = await _service.GetByIdAsync(id);
        return _mapper.Map<Media>(domain);
    }

    [HttpPost]
    public async Task<Media> Create([FromBody] MediaModel model)
    {
        var domain = _mapper.Map<Media>(model);
        return await _service.AddAsync(domain);
    }

    [HttpPatch]
    public async Task<OperationResult> Update(Guid id, [FromBody] UserCreationModel model)
    {
        var domain = _mapper.Map<Media>(model);
        return await _service.UpdateAsync(id, domain);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }
}
