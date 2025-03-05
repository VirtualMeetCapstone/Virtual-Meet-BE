using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Hubs;

public class ChatHub(IMessageService _service, IMapper _mapper) : Hub
{
    public async Task SendRoomMessage(MessageCreationModel model)
    {
        var domain = _mapper.Map<Message>(model);
        await _service.AddAsync(domain);
        await Clients.Group(model.RoomId.ToString() ?? string.Empty).SendAsync("ReceiveMessage", domain);
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", "System", $"User {Context.ConnectionId} joined room {roomId}.", DateTime.UtcNow);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", "System", $"User {Context.ConnectionId} left room {roomId}.", DateTime.UtcNow);
    }
}
