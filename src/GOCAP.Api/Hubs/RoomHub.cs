namespace GOCAP.Api.Hubs;

public class RoomHub : Hub
{
    private readonly ILogger<RoomHub> _logger;

    public RoomHub(ILogger<RoomHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinRoom(string userId, string roomId)
    {
        _logger.LogInformation("[INFO] User '{Username}' joined Room '{RoomId}'", userId, roomId);

        if (string.IsNullOrEmpty(roomId))
            throw new HubException("Room ID cannot be empty");

        // Initialize room if it doesn't exist
        if (!RoomStateManager.roomPeers.ContainsKey(roomId))
            RoomStateManager.roomPeers[roomId] = new List<PeerInfo>();

        // Kiểm tra nếu user đã tồn tại trong phòng
        var existingPeer = RoomStateManager.roomPeers[roomId].FirstOrDefault(p => p.UserName == userId);
        if (existingPeer != null)
        {
            // Xóa kết nối cũ
            _logger.LogInformation("[INFO] Removing old connection for user '{Username}'", userId);
            await Clients.Client(existingPeer.PeerId).SendAsync("Disconnect");
            await Clients.Group(roomId).SendAsync(
               "PeerDisconnected",
               Context.ConnectionId,
               RoomStateManager.roomPeers[roomId].Count
           );
            RoomStateManager.roomPeers[roomId].Remove(existingPeer);
        }

        // Cập nhật danh sách kết nối mới của user
        RoomStateManager.Users[Context.ConnectionId] = new UserInfo
        {
            Name = userId,
            RoomId = roomId
        };

        // Tạo thông tin peer mới
        var peerInfo = new PeerInfo
        {
            PeerId = Context.ConnectionId,
            UserName = string.IsNullOrEmpty(userId) ? "Anonymous" : userId
        };

        // Gửi danh sách peers hiện tại cho người mới
        await Clients.Caller.SendAsync("ExistingPeers", RoomStateManager.roomPeers[roomId]);

        // Thêm peer mới vào room
        RoomStateManager.roomPeers[roomId].Add(peerInfo);

        // Thêm kết nối mới vào nhóm SignalR
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveJoinNotification", userId);
        await SendRoomState(roomId, Context.ConnectionId);

        // Thông báo cho các peer khác về user mới
        foreach (var peer in RoomStateManager.roomPeers[roomId])
        {
            if (peer.PeerId != Context.ConnectionId)
            {
                await Clients.Client(peer.PeerId).SendAsync(
                    "NewPeer",
                    Context.ConnectionId,
                    peerInfo.UserName,
                    RoomStateManager.roomPeers[roomId].Count
                );
            }
        }

        _logger.LogInformation("[ROOM STATE] Danh sách peers trong Room '{RoomId}':", roomId);
        foreach (var peer in RoomStateManager.roomPeers[roomId])
        {
            _logger.LogInformation("- PeerId: {PeerId}, UserName: {UserName}", peer.PeerId, peer.UserName);
        }

    }


    public async Task LeaveRoom(string roomId)
    {
        if (RoomStateManager.roomPeers.ContainsKey(roomId))
        {
            var peer = RoomStateManager.roomPeers[roomId]
                .FirstOrDefault(p => p.PeerId == Context.ConnectionId);

            if (peer != null)
            {
                RoomStateManager.roomPeers[roomId].Remove(peer);

                await Clients.Group(roomId).SendAsync(
                    "PeerDisconnected",
                    Context.ConnectionId,
                    RoomStateManager.roomPeers[roomId].Count
                );

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

                if (RoomStateManager.roomPeers[roomId].Count == 0)
                {
                    RoomStateManager.roomPeers.TryRemove(roomId, out _);

                    // ✅ Xóa roomId khỏi SharingUsers nếu phòng trống
                    RoomStateManager.SharingUsers.TryRemove(roomId, out _);

                    await Clients.Group(roomId).SendAsync("ReceiveRoomState", new { Sharing = false });
                }
            }
        }
    }


    public async Task SendShare()
    {
        if (RoomStateManager.Users.TryGetValue(Context.ConnectionId, out UserInfo user))
        {
            _logger.LogInformation("🔁 [SHARE] {User} shared in Room {RoomId}", user.Name, user.RoomId);
            RoomStateManager.SharingUsers.TryAdd(user.RoomId,true);
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
        state.LastUpdated = DateTime.UtcNow; 

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

        // 🔥 Kiểm tra nếu phòng chỉ có 1 người và chưa có video
        if (GetRoomUserCount(roomId) == 1 && string.IsNullOrEmpty(state.VideoId))
        {
            state.VideoId = "dQw4w9WgXcQ"; // 🎵 Video mặc định (ví dụ: Rickroll 😆)
            state.Timestamp = 0;
            state.IsPaused = true;
            state.LastUpdated = DateTime.UtcNow;
        }

        double actualTime = state.Timestamp;
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
            Sharing = RoomStateManager.SharingUsers.ContainsKey(roomId)
        };

        _logger.LogInformation("📡 Sending room state for {RoomId}: {VideoId} at {Time}s", roomId, roomState.VideoId, roomState.Time);
        await Clients.Client(connectionId).SendAsync("ReceiveRoomState", roomState);
    }


    public static int GetRoomUserCount(string roomId)
    {
        if (RoomStateManager.roomPeers.TryGetValue(roomId, out var peers))
        {
            return peers.Count;
        }
        return 0;
    }


    //wRTC
    public async Task SendOffer(string targetPeerId, string offer)
    {
        // Find the sender's room and username
        string senderName = "Anonymous";
        foreach (var room in RoomStateManager.roomPeers)
        {
            var peer = room.Value.FirstOrDefault(p => p.PeerId == Context.ConnectionId);
            if (peer != null)
            {
                senderName = peer.UserName;
                break;
            }
        }

        await Clients.Client(targetPeerId).SendAsync(
            "ReceiveOffer",
            Context.ConnectionId,
            senderName,
            offer
        );
    }
    public async Task RequestStream(string targetPeerId)
    {
        await Clients.Client(targetPeerId).SendAsync("ReceiveStreamRequest", Context.ConnectionId);
    }


    public async Task SendAnswer(string targetPeerId, string answer)
    {
        await Clients.Client(targetPeerId).SendAsync(
            "ReceiveAnswer",
            Context.ConnectionId,
            answer
        );
    }

    public async Task SendCandidate(string targetPeerId, string candidate)
    {
        await Clients.Client(targetPeerId).SendAsync(
            "ReceiveCandidate",
            Context.ConnectionId,
            candidate
        );
    }

    // Handle disconnections
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Find which rooms the user is in
        foreach (var room in RoomStateManager.roomPeers)
        {
            await LeaveRoom(room.Key);
        }

        await base.OnDisconnectedAsync(exception);
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

