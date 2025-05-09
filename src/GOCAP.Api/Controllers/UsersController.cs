﻿using GOCAP.Database;
using Microsoft.EntityFrameworkCore;

namespace GOCAP.Api.Controllers;

[Route("users")]
public class UsersController(IUserService _userService,
    IUserBlockService _userBlockService,
    IFollowService _followService,
    AppSqlDbContext _dbContext,
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
    [AllowAnonymous]
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
    [Authorize]
    public async Task<OperationResult> FollowOrUnfollow([FromBody] FollowCreationModel model)
    {
        var follow = _mapper.Map<Follow>(model);
        var result = await _followService.FollowOrUnfollowAsync(follow);
        return result;
    }

    [HttpGet("is-following/{followingId}")]
    [Authorize]
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
 
    [HttpPost("check-block")]
    public async Task<ActionResult<bool>> GetBlockOrBlockedAsync([FromBody] UserBlockCreationModel model)
    {
        var domain = _mapper.Map<UserBlock>(model);
        var result = await _userBlockService.GetBlockOrBlockedAsync(domain);

        return Ok(result != null);
    }


    [HttpGet("search")]
    public async Task<List<UserSearchModel>> SearchUsers([FromQuery] string userName, [FromQuery] int limit = 10)
    {
        var users = await _userService.SearchUsersAsync(userName, limit);
        var results = _mapper.Map<List<UserSearchModel>>(users);
        return results;
    }

    [HttpGet("{userId}/vip-level")]
    public async Task<IActionResult> GetUserVipLevel([FromRoute] Guid userId)
    {
        var vipEntity = await _userService.GetUserVipLevelAsync(userId);
        var model = _mapper.Map<UserVipModel>(vipEntity);
        return Ok(model);
    }

    [HttpPost("{userId}/vip-level")]
    public async Task<IActionResult> AddOrUpdateUserVipLevel(
        [FromRoute] Guid userId,
        [FromBody] UserVipModel model)
    {
        var hasPaid = await _userService.HasUserPaidForVipAsync(userId, model.PackageId);
        if (!hasPaid)
        {
            return BadRequest("Bạn cần thanh toán trước khi nâng cấp VIP.");
        }

        await _userService.AddOrUpdateUserVipAsync(userId, model.PackageId, model.ExpireAt);
        return Ok();
    }

    [HttpGet("{userId}/blocked")]
    public async Task<List<UserBlockModel>> GetUserBlocks([FromRoute] Guid userId)
    {
        var userBlocks = await _userBlockService.GetUserBlocksAsync(userId);
        var results = _mapper.Map<List<UserBlockModel>>(userBlocks);
        return results;
    }

    [HttpGet("blocked-by/{userId}")]
    public async Task<List<UserBlockModel>> GetBlockedByUsers([FromRoute] Guid userId)
    {
        var blockedByUsers = await _userBlockService.GetBlockedByUsersAsync(userId);
        var results = _mapper.Map<List<UserBlockModel>>(blockedByUsers);
        return results;
    }

    [HttpGet("followings/{userId}")]
    public async Task<List<UserSearchModel>> GetFollowings([FromRoute] Guid userId)
    {
        var followings = await _dbContext.UserFollows
        .Where(uf => uf.FollowerId == userId)
        .Join(_dbContext.Users,
              uf => uf.FollowingId,
              u => u.Id,
              (uf, u) => new UserSearchModel
              {
                  Id = u.Id,
                  Name = u.Name,
                  Picture = JsonHelper.Deserialize<Media>(u.Picture),
              })
        .ToListAsync();

        return followings;
    }

    [HttpGet("followers/{userId}")]
    public async Task<List<UserSearchModel>> GetFollowers([FromRoute] Guid userId)
    {
        var followers = await _dbContext.UserFollows
            .Where(uf => uf.FollowingId == userId)
            .Join(_dbContext.Users,
                  uf => uf.FollowerId,
                  u => u.Id,
                  (uf, u) => new UserSearchModel
                  {
                      Id = u.Id,
                      Name = u.Name,
                      Picture = JsonHelper.Deserialize<Media>(u.Picture),
                  })
            .ToListAsync();

        return followers;
    }

    [HttpGet("friends/{userId}")]
    public async Task<List<UserSearchModel>> GetFriends([FromRoute] Guid userId)
    {
        var followingIds = _dbContext.UserFollows
        .Where(uf => uf.FollowerId == userId)
        .Select(uf => uf.FollowingId);

        var followerIds = _dbContext.UserFollows
            .Where(uf => uf.FollowingId == userId)
            .Select(uf => uf.FollowerId);

        var friendIds = await followingIds.Intersect(followerIds).ToListAsync();

        var friends = await _dbContext.Users
            .Where(u => friendIds.Contains(u.Id))
            .Select(u => new UserSearchModel
            {
                Id = u.Id,
                Name = u.Name,
                Picture = JsonHelper.Deserialize<Media>(u.Picture),
            })
            .ToListAsync();

        return friends;
    }

}
