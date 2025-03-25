namespace GOCAP.Api.Hubs;

public static class RoomStateManager
{
    public static ConcurrentDictionary<string, List<PeerInfo>> roomPeers = new();
    public static ConcurrentDictionary<string, VideoState> RoomStates { get; } = new();
    public static ConcurrentDictionary<string, UserInfo> Users { get; } = new();
    public static ConcurrentDictionary<string, bool> SharingUsers { get; } = new();
}

public class UserInfo
{
    public string Name { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
}

public class PeerInfo
{
    public string PeerId { get; set; }
    public string? UserId { get; set; }
    public string UserName { get; set; }
}

public class VideoState
{
    public string VideoId { get; set; } = "dQw4w9WgXcQ";
    public double Timestamp { get; set; } = 0; 
    public bool IsPaused { get; set; } = true;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
