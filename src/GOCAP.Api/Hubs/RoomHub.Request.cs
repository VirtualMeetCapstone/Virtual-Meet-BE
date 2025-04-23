namespace GOCAP.Api.Hubs;

public partial class RoomHub
{
    public async Task RequestJoinRoom(Guid roomId, Guid userId)
    {
        var redisKey = $"JoinRequest:{roomId}:{userId}";
        var existed = await _redisService.GetAsync<string>(redisKey);
        if (!string.IsNullOrEmpty(existed))
        {
            await Clients.Caller.SendAsync("JoinFailed", "You already requested to join this room");
            return;
        }

        // Lưu yêu cầu vào Redis với TTL 10 phút
        await _redisService.SetAsync(redisKey, "Pending", TimeSpan.FromMinutes(10));

        // Notify room owner
        var room = await _service.GetByIdAsync(roomId);
        if (room != null)
        {
            await Clients.User(room.OwnerId.ToString()).SendAsync("ReceiveJoinRequest", new { RoomId = roomId, UserId = userId });
        }
    }
    public async Task AcceptJoinRequest(string roomId, string userId)
    {
        var redisKey = $"JoinRequest:{roomId}:{userId}";
        var existed = await _redisService.GetAsync<string>(redisKey);
        if (string.IsNullOrEmpty(existed))
        {
            await Clients.Caller.SendAsync("JoinFailed", "No pending join request found");
            return;
        }

        // Xóa yêu cầu trong Redis
        await _redisService.DeleteAsync(redisKey);
        await JoinRoom(userId, roomId, "", true);

        // Thông báo cho người dùng về việc được chấp nhận
        await Clients.User(userId).SendAsync("JoinRoomSuccess", new { RoomId = roomId });
    }

    // Reject join request
    public async Task RejectJoinRequest(string roomId, string userId)
    {
        var redisKey = $"JoinRequest:{roomId}:{userId}";
        var existed = await _redisService.GetAsync<string>(redisKey);
        if (string.IsNullOrEmpty(existed))
        {
            await Clients.Caller.SendAsync("JoinFailed", "No pending join request found");
            return;
        }

        // Xóa yêu cầu trong Redis
        await _redisService.DeleteAsync(redisKey);

        // Thông báo cho người dùng về việc yêu cầu bị từ chối
        await Clients.User(userId).SendAsync("JoinRoomRejected", new { RoomId = roomId });
    }
}
