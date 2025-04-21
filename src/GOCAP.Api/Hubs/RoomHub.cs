namespace GOCAP.Api.Hubs;

public partial class RoomHub (
    ILogger<RoomHub> _logger,
    IUserService _userService, 
    IMessageService _service,
    IRedisService _redisService,
    IMessageReactionService _messageReactionService,
    IMapper _mapper ) : Hub
{

    public async Task JoinRoom(string userId, string roomId, string password = "")
    {
        var redisKey = $"Room:Password:{roomId}";
        var passwordHash = await _redisService.GetAsync<string>(redisKey);
        if (!string.IsNullOrEmpty(passwordHash))
        {
            var isValidPassword = BCrypt.Net.BCrypt.Verify(password, passwordHash);
            if (!isValidPassword)
            {
                _logger.LogWarning("User {UserId} provided incorrect password for room {RoomId}.", userId, roomId);
                await Clients.Caller.SendAsync("JoinFailed", "WrongPassword");
                return;
            }
        }

        _logger.LogInformation("[INFO] User '{Username}' joined Room '{RoomId}'", userId, roomId);

        if (string.IsNullOrEmpty(roomId))
            throw new HubException("Room ID cannot be empty");

        // Initialize room if it doesn't exist
        if (!RoomStateManager.RoomPeers.ContainsKey(roomId))
            RoomStateManager.RoomPeers[roomId] = [];

        // Kiểm tra nếu user đã tồn tại trong phòng
        var existingPeer = RoomStateManager.RoomPeers[roomId].FirstOrDefault(p => p.UserId == userId);
        if (existingPeer != null)
        {
            // Xóa kết nối cũ
            _logger.LogInformation("[INFO] Removing old connection for user '{Username}'", userId);
            await Clients.Client(existingPeer.PeerId ?? "").SendAsync("Disconnect");
            await Clients.Group(roomId).SendAsync(
               "PeerDisconnected",
               Context.ConnectionId,
               RoomStateManager.RoomPeers[roomId].Count
           );
            RoomStateManager.RoomPeers[roomId].Remove(existingPeer);
        }

        // Cập nhật danh sách kết nối mới của user
        RoomStateManager.Users[Context.ConnectionId] = new RoomConnectedUserModel
        {
            Name = userId,
            RoomId = roomId
        };

        Guid guidID = Guid.Parse(userId); // Chuyển đổi sang Guid
        var user = await _userService.GetUserProfileAsync(guidID);
        
        // Tạo thông tin peer mới
        var peerInfo = new RoomPeerModel
        {
            PeerId = Context.ConnectionId,
            UserName = userId,
            UserId = userId
        };

        // Gửi danh sách peers hiện tại cho người mới
        await Clients.Caller.SendAsync("ExistingPeers", RoomStateManager.RoomPeers[roomId]);
        await Clients.Caller.SendAsync("ConnectionId", Context.ConnectionId);

        // Thêm peer mới vào room
        RoomStateManager.RoomPeers[roomId].Add(peerInfo);

        // Thêm kết nối mới vào nhóm SignalR
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveJoinNotification", userId);
        await SendRoomState(roomId, Context.ConnectionId);

        // Thông báo cho các peer khác về user mới
        foreach (var peer in RoomStateManager.RoomPeers[roomId])
        {
            if (peer.PeerId != Context.ConnectionId)
            {
                await Clients.Client(peer.PeerId ?? "").SendAsync(
                    "NewPeer",
                    Context.ConnectionId,
                    peerInfo.UserName,
                    RoomStateManager.RoomPeers[roomId].Count
                );
            }
        }

        _logger.LogInformation("[ROOM STATE] Danh sách peers trong Room '{RoomId}':", roomId);
        foreach (var peer in RoomStateManager.RoomPeers[roomId])
        {
            _logger.LogInformation("- PeerId: {PeerId}, UserName: {UserName}", peer.PeerId, peer.UserName);
        }

    }

    public async Task MuteUser(string roomId, string targetUserId, bool muted)
    {
        await Clients.Group(roomId).SendAsync("HostMutedUser", new
        {
            userId = targetUserId,
            muted = muted
        });
    }

    public async Task MuteVideoUser(string roomId, string targetUserId, bool muted)
    {
        await Clients.Group(roomId).SendAsync("HostMutedVideoUser", new
        {
            userId = targetUserId,
            muted = muted
        });
    }

    public async Task KickUser(string roomId, string targetUserId, string reason)
    {
        await Clients.Group(roomId).SendAsync("HostKickUser", targetUserId, reason);
    }


    public async Task UpdateMicStatus(string roomId, string userId, bool isMicOn)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMicStatusUpdate", userId, isMicOn);
    }

    public async Task UpdateCameraStatus(string roomId, string userId, bool isCamOn)
    {
        await Clients.Group(roomId).SendAsync("ReceiveCameraStatusUpdate", userId, isCamOn);
    }


    public async Task LeaveRoom(string roomId)
    {
        if (RoomStateManager.RoomPeers.TryGetValue(roomId, out List<RoomPeerModel>? value))
        {
            var peer = value.FirstOrDefault(p => p.PeerId == Context.ConnectionId);

            if (peer != null)
            {
                RoomStateManager.RoomPeers[roomId].Remove(peer);

                await Clients.Group(roomId).SendAsync(
                    "PeerDisconnected",
                    Context.ConnectionId,
                    RoomStateManager.RoomPeers[roomId].Count
                );

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

                if (RoomStateManager.RoomPeers[roomId].Count == 0)
                {
                    RoomStateManager.RoomPeers.TryRemove(roomId, out _);

                    // ✅ Xóa roomId khỏi SharingUsers nếu phòng trống
                    RoomStateManager.SharingUsers.TryRemove(roomId, out _);

                    await Clients.Group(roomId).SendAsync("ReceiveRoomState", new { Sharing = false });
                }
            }
        }
    }

    public async Task SendShare()
    {
        if (RoomStateManager.Users.TryGetValue(Context.ConnectionId, out RoomConnectedUserModel? user))
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

    public async Task SendRaiseHand(string username, string roomId)
    {
        Console.WriteLine($"📢 Emotion Sent - User: {username}, Room: {roomId}");

        await Clients.OthersInGroup(roomId).SendAsync("ReceiveRaiseHand", username);
    }

    public async Task SendLowerHand(string username, string roomId)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveLowerHand", username);
    }

    public async Task SendEmotion(string username, string roomId, string type, double x, double y)
    {
        _logger.LogInformation("Sent - User: {Username}, Room: {RoomId}, Type: {Type}, Position: ({X}, {Y})",
            username, roomId, type, x, y);
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveEmotion", username, type, x, y);
    }


    public async Task SelectVideo(string roomId, string videoId)
    {
        var state = RoomStateManager.RoomStates.GetOrAdd(roomId, new RoomVideoStateModel());
        state.VideoId = videoId;
        state.Timestamp = 0;
        state.IsPaused = true;

        _logger.LogInformation("🎬 Room {RoomId} - Video selected: {VideoId}", roomId, videoId);
        await Clients.Group(roomId).SendAsync("ReceiveSelectedVideo", roomId, videoId, 0, true);
    }

    public async Task UpdatePlayerStatus(string roomId, int status, double time)
    {
        var state = RoomStateManager.RoomStates.GetOrAdd(roomId, new RoomVideoStateModel());

        state.Timestamp = time;
        state.IsPaused = (status == 2);
        state.LastUpdated = DateTime.UtcNow; 

        _logger.LogInformation("⏯️ Room {RoomId} - Status: {Status} | Time: {Time}s", roomId, status, time);
        await Clients.Group(roomId).SendAsync("receiveplayerstatus", roomId, status, time);
    }


    public async Task GetRoomState()
    {
        if (!RoomStateManager.Users.TryGetValue(Context.ConnectionId, out RoomConnectedUserModel? user))
        {
            _logger.LogError("❌ [ERROR] GetRoomState failed - User not found");
            return;
        }

        await SendRoomState(user.RoomId, Context.ConnectionId);
    }

    public async Task SendRoomState(string roomId, string connectionId)
    {
        var state = RoomStateManager.RoomStates.GetOrAdd(roomId, new RoomVideoStateModel());

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

        var roomState = new RoomPlaybackStateModel
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
        if (RoomStateManager.RoomPeers.TryGetValue(roomId, out var peers))
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
        foreach (var room in RoomStateManager.RoomPeers)
        {
            var peer = room.Value.FirstOrDefault(p => p.PeerId == Context.ConnectionId);
            if (peer != null)
            {
                senderName = peer.UserName ?? "";
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

    public async Task SendSubtitle(string roomId, string username, string subtitle, string sourceLang)
    {
        await Clients.Group(roomId).SendAsync("ReceiveSubtitle", username, subtitle, sourceLang);
    }

    // Handle disconnections
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Find which rooms the user is in
        foreach (var room in RoomStateManager.RoomPeers)
        {
            await LeaveRoom(room.Key);
        }

        await base.OnDisconnectedAsync(exception);
    }


}

