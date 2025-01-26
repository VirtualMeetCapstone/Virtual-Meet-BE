namespace GOCAP.Api.Controllers;

[Route("follows")]
[ApiController]
public class FollowController(IFollowService _service, IMapper _mapper) : ApiControllerBase
{

    [HttpPost]
    public async Task<OperationResult> FollowOrUnfollow([FromBody] FollowCreationModel model)
    {
        var follow = _mapper.Map<Follow>(model);
        var result = await _service.FollowAsync(follow);
        return result;
    }
}
