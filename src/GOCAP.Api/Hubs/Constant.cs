using System.Collections.Concurrent;

namespace GOCAP.Api.Hubs
{
    public class Constant
    {
        public class UserInfo
        {
            public string Name { get; set; } = string.Empty;
            public string RoomId { get; set; } = string.Empty;
        }

        public static class RoomState
        {
            public static HashSet<string> SharingUsers { get; } = new();
            public static ConcurrentDictionary<string, UserInfo> Users { get; } = new();
            public static (string VideoId, double Timestamp, bool IsPaused) VideoState { get; set; } = ("4Lq-I3xQxns", 0, true);
        }
    }
}
