namespace GOCAP.Api.Controllers;

[Route("rooms-favourite")]
[ApiController]
public class RoomFavouriteController(IRoomFavouriteService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpGet("page")]
    public async Task<QueryResult<RoomFavouriteModel>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetByPageAsync(queryInfo);
        var result = _mapper.Map<QueryResult<RoomFavouriteModel>>(domain);
        return result;
    }

    [HttpPost]
    public async Task<RoomFavouriteModel> Create([FromBody] RoomFavouriteCreationModel model)
    {
        var room = _mapper.Map<RoomFavourite>(model);
        var result = await _service.AddAsync(room);
        return _mapper.Map<RoomFavouriteModel>(result);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _service.DeleteByIdAsync(id);
    }
}
