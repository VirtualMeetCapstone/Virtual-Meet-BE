namespace GOCAP.Api.Controllers;

[Route("users")]
public class UsersController(IUserService _userService,
	IUserBlockService _userBlockService,
	IFollowService _followService,
	IMapper _mapper) : ApiControllerBase
{

	/// <summary>
	/// Get users by with paging.
	/// </summary>
	/// <param name="queryInfo"></param>
	/// <returns></returns>
	[HttpGet]
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
		var user = await _userService.GetUserProfileAsync(id);
		return _mapper.Map<UserProfileModel>(user);
	}

	/// <summary>
	/// Update user profile by id.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="model"></param>
	/// <returns></returns>
	[HttpPatch("{id}")]
	public async Task<OperationResult> UpdateUserProfile([FromRoute] Guid id, [FromForm] UserUpdationModel model)
	{
		var user = _mapper.Map<User>(model);
		user.Id = id;
		return await _userService.UpdateAsync(id, user);
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
		var result = await _followService.FollowOrUnfollowAsync(follow);
		return result;
	}

    [HttpGet("is-following/{followingId}")]
    public async Task<bool> IsFollowing([FromRoute] Guid followingId)
    {
		return await _followService.IsFollowingAsync(followingId);
    }

    /// <summary>
    /// Block or unblock another user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>OperationResult</returns>
    [HttpPost("block")]
	public async Task<OperationResult> BlockOrUnblock([FromBody] UserBlockCreationModel model)
	{
		var block = _mapper.Map<UserBlock>(model);
		var results = await _userBlockService.BlockOrUnblockAsync(block);
		return results;
	}

    /// <summary>
    /// Delete a user by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _userService.DeleteByIdAsync(id);
    }
	[HttpGet("{userId}/blocked")]
	public async Task<List<UserBlockModel>> GetUserBlocks([FromRoute] Guid userId)
	{
		var userBlocks = await _userBlockService.GetUserBlocksAsync(userId);
		var results = _mapper.Map<List<UserBlockModel>>(userBlocks);
		return results;
	}
	[HttpGet("search")]
	public async Task<List<UserSearchModel>> SearchUsers([FromQuery] string userName, [FromQuery] int limit = 10)
	{
		var users = await _userService.SearchUsersAsync(userName, limit);
		var results = _mapper.Map<List<UserSearchModel>>(users);
		return results;
	}
}
