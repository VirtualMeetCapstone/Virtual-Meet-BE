using System.Collections.Concurrent;

namespace GOCAP.Api.Hubs
{
    public class Constant
    {
        public static readonly ConcurrentDictionary<string, string> _userGroups = new();
        public static ConcurrentDictionary<string, (string VideoId, double Timestamp, bool IsPaused)> RoomVideoStates = new();
    }
}
