using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Hubs;

public class ChatHub(IMessageService _service) : Hub
{
    public async Task SendMessage(MessageType type, Guid? roomId, Guid? groupId, Guid senderId, Guid? receiverId, string content, List<Media>? attachments)
    {
        var messageBuilder = new MessageBuilder()
            .SetType(type)
            .SetRoomId(roomId)
            .SetGroupId(groupId)
            .SetSenderId(senderId)
            .SetReceiverId(receiverId)
            .SetContent(content);

        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                messageBuilder.AddAttachment(attachment);
            }
        }

        var message = messageBuilder.Build();
        await _service.AddAsync(message);

        if (type == MessageType.Room && roomId != null)
        {
            await Clients.Group(roomId.ToString() ?? string.Empty).SendAsync("ReceiveMessage", message);
        }
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", "System", $"User {Context.ConnectionId} đã tham gia room {roomId}.", DateTime.UtcNow);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", "System", $"User {Context.ConnectionId} đã rời khỏi room {roomId}.", DateTime.UtcNow);
    }
}
