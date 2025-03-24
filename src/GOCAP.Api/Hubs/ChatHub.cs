namespace GOCAP.Api.Hubs;

public class ChatHub(IMessageService _service, IMapper _mapper) : Hub
{
    private static readonly string ReceiveMessage = "ReceiveMessage";
    private static readonly string DeleteMessage = "DeleteMessage";
    private static readonly string UpdateMessage = "UpdateMessage";

    public async Task Send(Guid targetId, MessageType messageType, MessageCreationModel model)
    {
        var domain = _mapper.Map<Message>(model);
        domain.InitCreation();
        AssignTarget(domain, messageType, targetId);
        await BroadcastEvent(targetId, messageType, ReceiveMessage, domain);
        await _service.AddAsync(domain);
    }
    public async Task Delete(MessageType messageType, Guid targetId, Guid messageId)
    {
        await BroadcastEvent(targetId, messageType, DeleteMessage, messageId);
        await _service.DeleteByIdAsync(messageId);
    }

    public async Task Update(MessageType messageType, Guid targetId, Guid messageId, MessageCreationModel model
    ) 
    {
        var domain = _mapper.Map<Message>(model);
        domain.UpdateModify();
        domain.IsEdited = true;
        AssignTarget(domain, messageType, targetId);
        await BroadcastEvent(targetId, messageType, UpdateMessage, domain);
        await _service.UpdateAsync(messageId, domain);
    }
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        Console.WriteLine($"🔹 User {Context.ConnectionId} joined room {roomId}");
    }
    private async Task BroadcastEvent(Guid targetId, MessageType messageType, string eventName, object data)
    {
        switch (messageType)
        {
            case MessageType.Room:
            case MessageType.Group:
                await Clients.Group(targetId.ToString()).SendAsync(eventName, data);
                break;
            case MessageType.Direct:
                await Clients.User(targetId.ToString()).SendAsync(eventName, data);
                break;
        }
    }

    private static void AssignTarget(Message domain, MessageType messageType, Guid targetId)
    {
        domain.Type = messageType;
        switch (messageType)
        {
            case MessageType.Room:
                domain.RoomId = targetId;
                break;
            case MessageType.Direct:
                domain.ReceiverId = targetId;
                break;
            case MessageType.Group:
                domain.GroupId = targetId;
                break;
            default: throw new ParameterInvalidException();
        }
    }

}
