namespace GOCAP.Api.Controllers;

[Route("medias")]
[ApiController]
public class MediaController(IMediaService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<MediaModel>> GetAll()
    {
        var domain = await _service.GetAllAsync();
        return _mapper.Map<List<MediaModel>>(domain);
    }

    [HttpGet("page")]
    public async Task<QueryResult<MediaModel>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetByPageAsync(queryInfo);
        var result = _mapper.Map<QueryResult<MediaModel>>(domain);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<MediaModel> GetById([FromRoute] Guid id)
    {
        var domain = await _service.GetByIdAsync(id) ?? throw new ResourceNotFoundException("Media was not found.");
        return _mapper.Map<MediaModel>(domain);
    }

    [HttpPost]
    public async Task<MediaModel> Create([FromBody] MediaCreationModel model)
    {
        var media = _mapper.Map<Media>(model);
        var result = await _service.AddAsync(media);
        return _mapper.Map<MediaModel>(result);
    }

    [HttpPatch]
    public async Task<OperationResult> Update(Guid id, [FromBody] MediaCreationModel model)
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
