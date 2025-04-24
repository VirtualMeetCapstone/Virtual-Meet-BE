using GOCAP.Database;
using System.Collections.Concurrent;

namespace GOCAP.Api.Model;

public static class RoomStateManager
{
    public static readonly ConcurrentDictionary<string, List<RoomPeerModel>> RoomPeers = new();
    public static ConcurrentDictionary<string, RoomVideoStateModel> RoomStates { get; } = new();
    public static ConcurrentDictionary<string, RoomConnectedUserModel> Users { get; } = new();
    public static ConcurrentDictionary<string, bool> SharingUsers { get; } = new();
    public static ConcurrentDictionary<string, RoomStatistics> RoomStats { get; } = new();
}
