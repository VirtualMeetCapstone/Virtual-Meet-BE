namespace GOCAP.Api.Controllers;

[Route("users")]
[ApiController]
public class UserController(IUserService _userService, IMapper _mapper) : ApiControllerBase
{

    [HttpGet("{id}")]
    public async Task<UserProfileModel> GetUserProfile([FromRoute] Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return _mapper.Map<UserProfileModel>(user);
    }

    [HttpPatch]
    public async Task<OperationResult> UpdateUserProfile(Guid id, [FromForm] UserCreationModel model)
    {
        var user = _mapper.Map<User>(model);
        return await _userService.UpdateAsync(id, user);
    }
}
