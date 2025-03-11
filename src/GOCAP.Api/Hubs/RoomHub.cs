using System.Collections.Concurrent;

namespace GOCAP.Api.Hubs
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string RoomId { get; set; }
    }

    public class RoomHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserInfo> _users = new();


        public async Task JoinRoom(string username, string roomId)
        {
            // Lưu thông tin người dùng
            _users[Context.ConnectionId] = new UserInfo
            {
                Name = username,
                RoomId = roomId
            };

  
                Console.WriteLine($"👤 {username} vào phòng {roomId} - ConnectionId: {Context.ConnectionId}");

            // Kiểm tra client có thực sự trong Group không
            Constant._userGroups[Context.ConnectionId] = roomId; // Lưu Room của client

            Console.WriteLine($"🚀 {username} đã tham gia Room {roomId}");

            // Thêm vào group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            //// Thông báo cho phòng
            //await Clients.Group(roomId).SendAsync("UserJoined", username);

            // Nếu room đã có trạng thái video trước đó, gửi lại cho client mới

        }

        public async Task SendLike()
        {
            if (_users.TryGetValue(Context.ConnectionId, out UserInfo user))
            {
                // Gửi thông báo kèm tên
                await Clients.OthersInGroup(user.RoomId).SendAsync("ReceiveLike", user.Name);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Xóa thông tin khi ngắt kết nối
            _users.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendShare()
        {
            if (_users.TryGetValue(Context.ConnectionId, out UserInfo user))
            {
                // Gửi thông báo đến tất cả mọi người trong phòng, bao gồm cả người gửi
                await Clients.Group(user.RoomId).SendAsync("ReceiveShare", user.Name);
            }
        }
    }
}
