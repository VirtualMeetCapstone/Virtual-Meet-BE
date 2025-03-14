namespace GOCAP.Api.Hubs;

public class RoomHub : Hub
{
    private readonly ILogger<RoomHub> _logger;

    public RoomHub(ILogger<RoomHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinRoom(string username, string roomId)
    {
        _logger.LogInformation($"[INFO] User '{{username}}' joined Room '{{roomId}}'", username, roomId);

        Users[Context.ConnectionId] = new UserInfo { Name = username, RoomId = roomId };

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveJoinNotification", username);
    }

    public async Task SendLike()
    {
        if (Users.TryGetValue(Context.ConnectionId, out UserInfo user))
        {
            _logger.LogInformation("❤️ [LIKE] {User} sent a like in Room {RoomId}", user.Name, user.RoomId);
            await Clients.OthersInGroup(user.RoomId).SendAsync("ReceiveLike", user.Name);
        }
        else
        {
            _logger.LogError("❌ [ERROR] SendLike failed - User not found for ConnectionId {ConnectionId}", Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Users.TryRemove(Context.ConnectionId, out UserInfo user))
        {
            _logger.LogWarning("❌ [DISCONNECTED] {User} left Room {RoomId}", user.Name, user.RoomId);
        }
        else
        {
            _logger.LogWarning("⚠️ [WARNING] Unknown user disconnected - ConnectionId: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendShare()
    {
        if (Users.TryGetValue(Context.ConnectionId, out UserInfo user))
        {
            _logger.LogInformation("🔁 [SHARE] {User} shared in Room {RoomId}", user.Name, user.RoomId);
            SharingUsers.Add(user.RoomId);
            await Clients.Group(user.RoomId).SendAsync("ReceiveShare", user.Name);
        }
        else
        {
            _logger.LogError("❌ [ERROR] SendShare failed - User not found for ConnectionId {ConnectionId}", Context.ConnectionId);
        }
    }

    public async Task SelectVideo(string roomId, string videoId)
    {
        _logger.LogInformation("🎬 Room {RoomId} - Video selected: {VideoId}", roomId, videoId);
        VideoState = (videoId, 0, VideoState.IsPaused);
        await Clients.Group(roomId).SendAsync("ReceiveSelectedVideo", roomId, videoId, 0, VideoState.IsPaused);
    }

    public async Task UpdatePlayerStatus(string roomId, int status, double time)
    {
        VideoState = (VideoState.VideoId, time, status == 2);
        _logger.LogInformation("⏯️ Room {RoomId} - Video: {VideoId} | Status: {Status} | Time: {Time}", roomId, VideoState.VideoId, status, time);
        await Clients.Group(roomId).SendAsync("receiveplayerstatus", roomId, status, time);
    }

    public async Task GetRoomState()
    {
        if (Users.TryGetValue(Context.ConnectionId, out UserInfo user))
        {
            string roomId = user.RoomId;
            _logger.LogInformation("📡 [SYNC] Sending room state to {User} - Room {RoomId}", user.Name, roomId);

            if (!string.IsNullOrEmpty(VideoState.VideoId))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveSelectedVideo", roomId, VideoState.VideoId, VideoState.Timestamp, VideoState.IsPaused);
            }

            if (SharingUsers.Contains(roomId))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveShare", user.Name);
            }
        }
    }
}
