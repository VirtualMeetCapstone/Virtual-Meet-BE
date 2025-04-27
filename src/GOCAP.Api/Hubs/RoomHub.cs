using GOCAP.Database;
using GOCAP.Repository;
using static GOCAP.Api.Model.RoomPollModel;

namespace GOCAP.Api.Hubs;

public partial class RoomHub(
    ILogger<RoomHub> _logger,
    IUserService _userService,
    IRoomService _service,
    IMessageService _messageService,
    IRedisService _redisService,
    IRoomMemberRepository _roomMemberRepository,
    IMessageReactionService _messageReactionService,
    IAIService aIService,
    IMapper _mapper,
    AppMongoDbContext _dbContext,
    IServiceScopeFactory _scopeFactory) : Hub 
{

    public async Task JoinRoom(string userId, string roomId, string password = "", bool isAccepted = false)
    {
        if (!isAccepted)
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

        var polls = RoomPollModel.PollManager.GetPollsForRoom(roomId);
        await Clients.Caller.SendAsync("PollUpdated", polls);

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
        _ = Task.Run(async () =>
        {
            try
            {
                // Use await using instead of using for proper async disposal
                await using var scope = _scopeFactory.CreateAsyncScope();
                var scopedUserService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var scopedRoomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                var scopedDbContext = scope.ServiceProvider.GetRequiredService<AppMongoDbContext>();
                var scopedMapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var scopedRoomMemberRepository = scope.ServiceProvider.GetRequiredService<IRoomMemberRepository>();
                var guidID = Guid.Parse(userId);
                var user = await scopedUserService.GetUserProfileAsync(guidID);

                var roomMemberEntity = new Database.RoomMemberEntity
                {
                    Id = Guid.NewGuid(),
                    RoomId = Guid.Parse(roomId),
                    UserId = Guid.Parse(userId),
                    CreateTime = DateTime.Now.Ticks,
                    LastModifyTime = DateTime.Now.Ticks
                };
                await scopedRoomMemberRepository.AddAsync(roomMemberEntity);
                var userInfo = new UserInfo
                {
                    Id = userId,
                    Name = user?.Name ?? userId,
                    ImageUrl = user?.Picture?.Url ?? ""
                };

                await UpdateRoomStatisticsOnJoinAsync(roomId, userInfo, scopedRoomService, scopedDbContext, scopedMapper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating statistics for user {UserId}", userId);
            }
        });

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
        if (!RoomStateManager.RoomPeers.TryGetValue(roomId, out var peers))
            return;

        var peer = peers.FirstOrDefault(p => p.PeerId == Context.ConnectionId);
        if (peer == null)
            return;

        RoomStateManager.RoomPeers[roomId].Remove(peer);

        // These SignalR calls are safe here
        await Clients.Group(roomId).SendAsync(
            "PeerDisconnected",
            Context.ConnectionId,
            RoomStateManager.RoomPeers[roomId].Count
        );
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        bool isRoomEmpty = RoomStateManager.RoomPeers[roomId].Count == 0;

      
        if (isRoomEmpty)
        {
            await Clients.All.SendAsync("RoomDeleted", roomId);
            await Clients.Group(roomId).SendAsync("ReceiveRoomState", new { Sharing = false });
        }

    
        string userId = peer.UserId;

        _ = Task.Run(async () =>
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var scopedUserService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var scopedRoomMemberRepo = scope.ServiceProvider.GetRequiredService<IRoomMemberRepository>();
                var scopedRoomService = scope.ServiceProvider.GetRequiredService<IRoomService>();

                var isLastPeerOfUser = !RoomStateManager.RoomPeers[roomId]
                    .Any(p => p.UserId == userId && p.PeerId != peer.PeerId);

                if (isLastPeerOfUser && userId != null)
                {
                    var user = await scopedUserService.GetUserProfileAsync(Guid.Parse(userId));
                    var userInfo = new UserInfo
                    {
                        Id = userId,
                        Name = user?.Name ?? userId,
                        ImageUrl = user?.Picture?.Url ?? ""
                    };
                    await UpdateRoomStatisticsOnLeaveAsync(roomId, userInfo);
                }

                if (isRoomEmpty)
                {
                   
                    RoomStateManager.RoomPeers.TryRemove(roomId, out _);
                    await scopedRoomMemberRepo.DeleteByRoomIdAsync(Guid.Parse(roomId));
                    await scopedRoomService.DeleteByIdAsync(Guid.Parse(roomId));
                    RoomStateManager.SharingUsers.TryRemove(roomId, out _);
                    _subtitleCache.TryRemove(roomId, out _); // Fixed the syntax here
                    RoomStateManager.RoomStats.TryRemove(roomId, out _);
                    var now = DateTime.UtcNow;
                    RemoveExpiredPolls(now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during LeaveRoom cleanup for room {RoomId}", roomId);
            }
        });
    }


    public static int GetRoomUserCount(string roomId)
    {
        if (RoomStateManager.RoomPeers.TryGetValue(roomId, out var peers))
        {
            return peers.Count;
        }
        return 0;
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

