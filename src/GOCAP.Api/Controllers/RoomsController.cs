﻿using GOCAP.Api.Model;
using GOCAP.Messaging.Producer;
﻿using GOCAP.Messaging.Producer;
using GOCAP.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace GOCAP.Api.Controllers;

[Route("rooms")]
public class RoomsController(IRoomService _service,
    IRoomFavouriteService _roomFavouriteService,
    IRedisService _redisService,
    IKafkaProducer _kafkaProducer,
    IUserContextService _userContextService,
    IHubContext<RoomListHub> _hubContext,
    IRoomMemberRepository roomMemberRepository,
    IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Get rooms with paging.
    /// </summary>
    /// <param name="queryInfo"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<QueryResult<RoomModel>> GetWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var domain = await _service.GetWithPagingAsync(queryInfo);
        var result = _mapper.Map<QueryResult<RoomModel>>(domain);
        return result;
    }

    /// <summary>
    /// Get room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<RoomModel> GetById([FromRoute] Guid id)
    {
        var domain = await _service.GetDetailByIdAsync(id);
        return _mapper.Map<RoomModel>(domain);
    }

    /// <summary>
    /// Create a new room.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<RoomModel> Create([FromBody] RoomCreationModel model)
    {
        var room = _mapper.Map<Room>(model);
        var result = await _service.AddAsync(room);
        var roomModel = _mapper.Map<RoomModel>(result);

        // 🔥 Push sự kiện real-time cho tất cả client
        await _hubContext.Clients.Group("RoomsPage").SendAsync("RoomCreated", roomModel);

        return roomModel;
    }

    /// <summary>
    /// Update a room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [ValidateModel]
    [Authorize]
    public async Task<OperationResult> Update([FromRoute] Guid id, [FromBody] RoomUpdationModel model)
    {
        var domain = _mapper.Map<Room>(model);
        domain.Id = id;
        var result = await _service.UpdateAsync(id, domain);

        // Nếu update thành công thì push real-time
        if (result.Success)
        {
            // Map lại RoomModel để client nhận
            var updatedRoomModel = _mapper.Map<RoomModel>(domain);
            await _hubContext.Clients.Group("RoomsPage").SendAsync("RoomUpdated", updatedRoomModel);
        }

        return result;
    }

    /// <summary>
    /// Delete a room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        var result = await _service.DeleteByIdAsync(id);

        // Nếu xoá thành công thì push real-time
        if (result.Success)
        {
            await _hubContext.Clients.Group("RoomsPage").SendAsync("RoomDeleted", id);
        }

        return result;
    }

    /// <summary>
    /// Create or delete a room favourite.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{roomId}/favourite")]
    public async Task<OperationResult> CreateOrDeleteRoomFavourite([FromRoute] Guid roomId, [FromBody] RoomFavouriteCreationModel model)
    {
        var room = _mapper.Map<RoomFavourite>(model);
        room.RoomId = roomId;
        var result = await _roomFavouriteService.CreateOrDeleteAsync(room);
        return result;
    }

    [HttpGet("{userId}/favourite")]
    public async Task<QueryResult<RoomFavouriteDetailModel>> GetRoomFavouritesByUserIdWithPaging([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var domain = await _roomFavouriteService.GetFavouritesByUserIdWithPagingAsync(userId, queryInfo);
        var result = _mapper.Map<QueryResult<RoomFavouriteDetailModel>>(domain);
        return result;
    }

    [HttpPost("check-password")]
    public async Task<IActionResult> CheckRoomPassword([FromBody] CheckRoomPasswordRequest request)
    {
        var redisKey = $"Room:Password:{request.RoomId}";
        var passwordHash = await _redisService.GetAsync<string>(redisKey);

        if (string.IsNullOrEmpty(passwordHash))
            return NotFound("Room password not found");

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);

        if (!isValid)
            return Unauthorized("Incorrect password");

        return Ok("Password is correct");
    }

    [HttpPost("{roomId}/send-summary-mail")]
    public async Task<OperationResult> SendEmailToRoomMember([FromRoute] Guid roomId, [FromQuery] string subject, [FromQuery] string content)
    {
        var metadata = new Dictionary<string, string>
        {
            { "RoomId", roomId.ToString() },
            { "Subject", subject },
            { "Content", content }
        };

        var notificationEvent = new NotificationEvent
        {
            Type = NotificationType.System,
            ActionType = ActionType.Add,
            Metadata = metadata
        };

        await _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, notificationEvent);

        return new OperationResult(true);
    }

    [HttpGet("{roomId}/member-info")]
    public async Task<IActionResult> GetRoomMemberInfo([FromRoute] Guid roomId)
    {
        var room = await _service.GetDetailByIdAsync(roomId);
        if (room == null)
            return NotFound();

        var count = await roomMemberRepository.CountByRoomIdAsync(roomId);

        return Ok(new
        {
            RoomId = roomId,
            CurrentCount = count,
            MaxCount = room.MaximumMembers
        });
    }
}

public class CheckRoomPasswordRequest
{
    public Guid RoomId { get; set; }
    public string Password { get; set; }
}