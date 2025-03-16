namespace GOCAP.Api.Hubs;

public static class RoomStateManager
{
    public static ConcurrentDictionary<string, VideoState> RoomStates { get; } = new();
    public static ConcurrentDictionary<string, UserInfo> Users { get; } = new();
    public static ConcurrentBag<string> SharingUsers { get; } = new();
}

public class UserInfo
{
    public string Name { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
}

public class VideoState
{
    public string VideoId { get; set; } = "dQw4w9WgXcQ";
    public double Timestamp { get; set; }
    public bool IsPaused { get; set; } = true;
}