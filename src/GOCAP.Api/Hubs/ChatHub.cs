using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Hubs;

public class ChatHub(IMessageService _service, IMapper _mapper) : Hub
{
    public async Task SendRoomMessage(RoomMessageCreationModel model)
    {
        var domain = _mapper.Map<RoomMessage>(model);
        await _service.AddAsync(domain);
        await Clients.Group(model.RoomId.ToString() ?? string.Empty).SendAsync("ReceiveMessage", domain);
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
