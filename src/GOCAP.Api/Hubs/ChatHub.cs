namespace GOCAP.Api.Hubs;

public class ChatHub(IMessageService _service, IMapper _mapper) : Hub
{
    private static readonly string ReceiveMessage = "ReceiveMessage";

    [ValidateModel]
    public async Task SendMessage([FromBody] MessageCreationModel model)
    {
        var domain = _mapper.Map<Message>(model);
        domain.InitCreation();
        await BroadcastMessage(model, ReceiveMessage, domain);
        await _service.AddAsync(domain);
    }

    public async Task EditMessage([FromRoute] Guid id, [FromBody] MessageCreationModel model)
    {
        var domain = _mapper.Map<Message>(model);
        var result = await _service.UpdateAsync(id, domain);
        if (result != null && result.Success)
        {
            await BroadcastMessage(model, "EditMessage", result);
        }
    }

    public async Task DeleteMessage([FromBody] MessageDeletionModel model)
    {
        var result = await _service.DeleteByIdAsync(model.Id);
        if (result!= null && result.Success) 
        {
            await BroadcastMessage(model, "RemoveMessage", model.Id);
        }
    }

    public async Task JoinRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).SendAsync(ReceiveMessage, "System", $"User {Context.ConnectionId} joined room {roomId}.", DateTime.UtcNow);
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).SendAsync(ReceiveMessage, "System", $"User {Context.ConnectionId} left room {roomId}.", DateTime.UtcNow);
    }

    private async Task BroadcastMessage(MessageBaseModel model, string action, object response)
    {
        var groupId = model.Type switch
        {
            MessageType.Direct => model.ReceiverId?.ToString(),
            MessageType.Room => model.RoomId?.ToString(),
            MessageType.Group => model.GroupId?.ToString(),
            _ => null
        };

        if (!string.IsNullOrEmpty(groupId))
        {
            await Clients.Groups(groupId).SendAsync(action, response);
        }
    }

}
