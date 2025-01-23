namespace GOCAP.Api.Controllers;

[Route("rooms")]
[ApiController]
public class RoomController(IRoomService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpGet("page")]
    public async Task<QueryResult<RoomModel>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetByPageAsync(queryInfo);
        var result = _mapper.Map<QueryResult<RoomModel>>(domain);
        return result;
    }

    [HttpPost]
    public async Task<RoomModel> Create([FromForm] RoomCreationModel model)
    {
        var room = _mapper.Map<Room>(model);
        var result = await _service.AddAsync(room);
        return _mapper.Map<RoomModel>(result);
    }

    [HttpPatch]
    public async Task<OperationResult> Update(Guid id, [FromBody] RoomCreationModel model)
    {
        var domain = _mapper.Map<Room>(model);
        return await _service.UpdateAsync(id, domain);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }
}
