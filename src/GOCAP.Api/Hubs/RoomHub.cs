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

        if (string.IsNullOrEmpty(roomId))
            throw new HubException("Room ID cannot be empty");

        // Initialize room if it doesn't exist
        if (!RoomStateManager.roomPeers.ContainsKey(roomId))
            RoomStateManager.roomPeers[roomId] = new List<PeerInfo>();

        RoomStateManager.Users[Context.ConnectionId] = new UserInfo
        {
            Name = username,
            RoomId = roomId
        };
        // Create peer info object
        var peerInfo = new PeerInfo
        {
            PeerId = Context.ConnectionId,
            UserName = string.IsNullOrEmpty(username) ? "Anonymous" : username
        };

        // Send existing peers list to new participant
        await Clients.Caller.SendAsync("ExistingPeers", RoomStateManager.roomPeers[roomId]);
        // Add new peer to room
        RoomStateManager.roomPeers[roomId].Add(peerInfo);

        // Add connection to SignalR group


        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveJoinNotification", username);
        await SendRoomState(roomId, Context.ConnectionId);


        // Notify existing peers about the new participant
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
    }

    public async Task LeaveRoom(string roomId)
    {
        if (RoomStateManager.roomPeers.ContainsKey(roomId))
        {
            // Find and remove the peer
            var peer = RoomStateManager.roomPeers[roomId].FirstOrDefault(p => p.PeerId == Context.ConnectionId);
            if (peer != null)
            {
                RoomStateManager.roomPeers[roomId].Remove(peer);

                // Notify other peers
                await Clients.Group(roomId).SendAsync(
                    "PeerDisconnected",
                    Context.ConnectionId,
                    RoomStateManager.roomPeers[roomId].Count
                );

                // Remove from SignalR group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

                // Remove the room if empty
                if (RoomStateManager.roomPeers[roomId].Count == 0)
                {
                    RoomStateManager.roomPeers.TryRemove(roomId, out _);
                }
            }
        }
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

