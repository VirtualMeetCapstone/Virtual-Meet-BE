namespace GOCAP.Api.Controllers;

[Route("users")]
public class UserController(IUserService _userService,
    IFollowService _followService,
    IMapper _mapper) : ApiControllerBase
{

    /// <summary>
    /// Get users by with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<QueryResult<UserModel>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _userService.GetByPageAsync(queryInfo);
        var result = _mapper.Map<QueryResult<UserModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get user profile by user id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<UserProfileModel> GetUserProfile([FromRoute] Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return _mapper.Map<UserProfileModel>(user);
    }

    /// <summary>
    /// Update user profile by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch]
    public async Task<OperationResult> UpdateUserProfile(Guid id, [FromForm] UserCreationModel model)
    {
        var user = _mapper.Map<User>(model);
        return await _userService.UpdateAsync(id, user);
    }

    /// <summary>
    /// Get notifications by user id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("notifications/{userId}")]
    public async Task<List<UserNotificationModel>> GetNotificationsByUserId([FromRoute] Guid userId)
    {
        var userNotifications = await _userService.GetNotificationsByUserIdAsync(userId);
        return _mapper.Map<List<UserNotificationModel>>(userNotifications);
    }

    /// <summary>
    /// Follow or unfollow another user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("follow")]
    public async Task<OperationResult> FollowOrUnfollow([FromBody] FollowCreationModel model)
    {
        var follow = _mapper.Map<Follow>(model);
        var result = await _followService.FollowAsync(follow);
        return result;
    }
}
