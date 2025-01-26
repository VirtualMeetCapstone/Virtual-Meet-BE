namespace GOCAP.Api.Controllers;

[Route("rooms-favourite")]
[ApiController]
public class RoomFavouriteController(IRoomFavouriteService _service, IMapper _mapper) : ApiControllerBase
{
    [HttpPost]
    public async Task<OperationResult> CreateOrDelete([FromBody] RoomFavouriteCreationModel model)
    {
        var room = _mapper.Map<RoomFavourite>(model);
        var result = await _service.CreateOrDeleteAsync(room);
        return result;
    }
}
