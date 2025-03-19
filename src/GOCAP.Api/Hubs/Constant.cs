namespace GOCAP.Api.Hubs;

public static class RoomStateManager
{
    public static ConcurrentDictionary<string, List<PeerInfo>> roomPeers = new ConcurrentDictionary<string, List<PeerInfo>>();
    public static ConcurrentDictionary<string, VideoState> RoomStates { get; } = new();
    public static ConcurrentDictionary<string, UserInfo> Users { get; } = new();
    public static ConcurrentBag<string> SharingUsers { get; } = new();
}

public class UserInfo
{
    public string Name { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
}

public class PeerInfo
{
    public string PeerId { get; set; }
    public string UserName { get; set; }
}

public class VideoState
{
    public string VideoId { get; set; } = "dQw4w9WgXcQ";
    public double Timestamp { get; set; } = 0; // Lưu thời gian video dừng/chạy
    public bool IsPaused { get; set; } = true; // Trạng thái pause/play
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // 🔥 Lưu thời điểm cuối cùng cập nhật
}
