namespace GOCAP.Api.Hubs
{
    public partial class RoomHub
    {
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

    }
}
