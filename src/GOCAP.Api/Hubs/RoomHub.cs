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
        _logger.LogInformation("[INFO] User '{Username}' joined Room '{RoomId}'", username, roomId);

        RoomStateManager.Users[Context.ConnectionId] = new UserInfo
        {
            Name = username,
            RoomId = roomId
        };

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveJoinNotification", username);
        await SendRoomState(roomId, Context.ConnectionId);
    }
    public async Task SendShare()
    {
        if (RoomStateManager.Users.TryGetValue(Context.ConnectionId, out UserInfo user))
        {
            _logger.LogInformation("🔁 [SHARE] {User} shared in Room {RoomId}", user.Name, user.RoomId);
            RoomStateManager.SharingUsers.Add(user.RoomId);
            await Clients.Group(user.RoomId).SendAsync("ReceiveShare", user.Name);
        }
        else
        {
            _logger.LogError("❌ [ERROR] SendShare failed - User not found for ConnectionId {ConnectionId}", Context.ConnectionId);
        }
    }

    public async Task SelectVideo(string roomId, string videoId)
    {
        var state = RoomStateManager.RoomStates.GetOrAdd(roomId, new VideoState());
        state.VideoId = videoId;
        state.Timestamp = 0;
        state.IsPaused = true;

        _logger.LogInformation("🎬 Room {RoomId} - Video selected: {VideoId}", roomId, videoId);
        await Clients.Group(roomId).SendAsync("ReceiveSelectedVideo", roomId, videoId, 0, true);
    }

    public async Task UpdatePlayerStatus(string roomId, int status, double time)
    {
        var state = RoomStateManager.RoomStates.GetOrAdd(roomId, new VideoState());

        state.Timestamp = time;
        state.IsPaused = (status == 2);
        state.LastUpdated = DateTime.UtcNow; // 🔥 Lưu thời gian cập nhật trạng thái

        _logger.LogInformation("⏯️ Room {RoomId} - Status: {Status} | Time: {Time}s", roomId, status, time);
        await Clients.Group(roomId).SendAsync("receiveplayerstatus", roomId, status, time);
    }


    public async Task GetRoomState()
    {
        if (!RoomStateManager.Users.TryGetValue(Context.ConnectionId, out UserInfo user))
        {
            _logger.LogError("❌ [ERROR] GetRoomState failed - User not found");
            return;
        }

        await SendRoomState(user.RoomId, Context.ConnectionId);
    }

    public async Task SendRoomState(string roomId, string connectionId)
    {
        var state = RoomStateManager.RoomStates.GetOrAdd(roomId, new VideoState());

        double actualTime = state.Timestamp; // Mặc định lấy timestamp đã lưu
        if (!state.IsPaused)
        {
            // 🔥 Nếu video đang chạy, tính thời gian thực tế
            actualTime += (DateTime.UtcNow - state.LastUpdated).TotalSeconds;
        }

        var roomState = new RoomState
        {
            VideoId = state.VideoId,
            Time = actualTime, // 🔥 Thời gian thực tế
            IsPaused = state.IsPaused,
            Sharing = RoomStateManager.SharingUsers.Contains(roomId)
        };

        _logger.LogInformation("📡 Sending room state for {RoomId}: {VideoId} at {Time}s", roomId, roomState.VideoId, roomState.Time);
        await Clients.Client(connectionId).SendAsync("ReceiveRoomState", roomState);
    }


    public class RoomState
    {
        [JsonProperty("videoId")]
        public string VideoId { get; set; } = "";
        [JsonProperty("time")]
        public double Time { get; set; }
        [JsonProperty("isPaused")]
        public bool IsPaused { get; set; } = true;
        [JsonProperty("sharing")]
        public bool Sharing { get; set; } = false;
    }

}

