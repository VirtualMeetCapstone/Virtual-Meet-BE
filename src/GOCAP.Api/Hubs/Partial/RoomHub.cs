namespace GOCAP.Api.Hubs;

public partial class RoomHub
{
    private static readonly string ReceiveMessageRoom = "ReceiveMessage";
    private static readonly string DeleteMessageRoom = "DeleteMessage";
    private static readonly string UpdateMessageRoom = "UpdateMessage";

    public async Task SendMessage(Guid roomId, MessageCreationModel model)
    {
        if (model == null || roomId == Guid.Empty)
        {
            throw new ParameterInvalidException();
        }
        
        try
        {
            var domain = _mapper.Map<Message>(model);
            domain.InitCreation();
            domain.RoomId = roomId;
            domain.Type = MessageType.Room;
            var result = _mapper.Map<MessageModel>(domain);
            await BroadcastEvent(roomId, ReceiveMessageRoom, result);
            await _service.AddAsync(domain);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", "Failed to send message: " + ex.Message);
            throw;
        }
    }

    public async Task DeleteMessage(Guid roomId, Guid messageId)
    {
        if (messageId == Guid.Empty || roomId == Guid.Empty)
        {
            throw new ParameterInvalidException();
        }

        try
        {
            await BroadcastEvent(roomId, DeleteMessageRoom, messageId);
            await _service.DeleteByIdAsync(messageId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", "Failed to send message: " + ex.Message);
            throw;
        }
    }

    public async Task UpdateMessage(Guid roomId, Guid messageId, MessageCreationModel model)
    {
        if (messageId == Guid.Empty || roomId == Guid.Empty || model == null)
        {
            throw new ParameterInvalidException();
        }
        try
        {
            var domain = _mapper.Map<Message>(model);
            domain.Id = messageId;
            domain.UpdateModify();
            domain.IsEdited = true;
            domain.RoomId = roomId;
            domain.Type = MessageType.Room;
            var result = _mapper.Map<MessageModel>(domain);
            await BroadcastEvent(roomId, UpdateMessageRoom, result);
            await _service.UpdateAsync(messageId, domain);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", "Failed to send message: " + ex.Message);
            throw;
        }
    }

    private async Task BroadcastEvent(Guid roomId, string eventName, object data)
    {
        await Clients.Group(roomId.ToString()).SendAsync(eventName, data);
    }
}