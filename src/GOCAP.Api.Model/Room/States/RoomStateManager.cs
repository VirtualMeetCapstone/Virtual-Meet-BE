using System.Collections.Concurrent;

namespace GOCAP.Api.Model;

public static class RoomStateManager
{
    public static ConcurrentDictionary<string, List<RoomPeerModel>> roomPeers = new();
    public static ConcurrentDictionary<string, RoomVideoStateModel> RoomStates { get; } = new();
    public static ConcurrentDictionary<string, RoomConnectedUserModel> Users { get; } = new();
    public static ConcurrentDictionary<string, bool> SharingUsers { get; } = new();
}
