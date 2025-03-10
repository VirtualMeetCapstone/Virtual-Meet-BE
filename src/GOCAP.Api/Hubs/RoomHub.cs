using System.Collections.Concurrent;

namespace GOCAP.Api.Hubs
{
    public class RoomHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> RoomUsers = new();

        public async Task JoinRoom(string roomId)
        {
            string connectionId = Context.ConnectionId;

            if (!RoomUsers.ContainsKey(roomId))
                RoomUsers[roomId] = new HashSet<string>();

            RoomUsers[roomId].Add(connectionId);

            Console.WriteLine($"📢 {connectionId} joined Room {roomId}");

            //Gửi danh sách user trong room cho tất cả client trong room
            await Clients.Group(roomId).SendAsync("ReceiveUserList", RoomUsers[roomId].ToList());

            await Groups.AddToGroupAsync(connectionId, roomId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string disconnectedRoom = null;

            foreach (var room in RoomUsers)
            {
                if (room.Value.Contains(Context.ConnectionId))
                {
                    room.Value.Remove(Context.ConnectionId);
                    disconnectedRoom = room.Key;

                    if (!room.Value.Any())
                        RoomUsers.TryRemove(room.Key, out _);

                    break;
                }
            }

            if (disconnectedRoom != null)
            {
                Console.WriteLine($"❌ {Context.ConnectionId} left Room {disconnectedRoom}");
                await Clients.Group(disconnectedRoom).SendAsync("ReceiveUserList", RoomUsers[disconnectedRoom]?.ToList() ?? new List<string>());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

