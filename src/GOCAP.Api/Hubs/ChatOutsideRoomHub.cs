using DocumentFormat.OpenXml.Spreadsheet;

namespace GOCAP.Api.Hubs
{
    public class ChatOutsideRoomHub:Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task AddToGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        public async Task SendPrivateMessage(string senderId, string receiverId, string message)
        {
            await Clients.Group(receiverId).SendAsync("ReceivePrivateMessage", senderId, message);
            await Clients.Group(senderId).SendAsync("ReceivePrivateMessage", senderId, message);
        }
        public async Task UserIsTyping(string senderId, string receiverId)
        {
            await Clients.Group(receiverId).SendAsync("UserTyping", senderId);
        }

    }
}
