namespace GOCAP.Api.Hubs;

public partial class RoomHub
{
    private static readonly string ReceiveMessageRoom = "ReceiveMessage";
    private static readonly string DeleteMessageRoom = "DeleteMessage";
    private static readonly string UpdateMessageRoom = "UpdateMessage";

    private static readonly string ReceiveMessageReactionRoom = "ReceiveMessageReaction";
    private static readonly string DeleteMessageReactionRoom = "DeleteMessageReaction";
    private static readonly string UpdateMessageReactionRoom = "UpdateMessageReaction";

    public async Task SendMessage(Guid roomId, MessageCreationModel model)
    {
        await ExecuteWithErrorHandling(async () =>
        {
            var result = new MessageModel
            {
                Id = Guid.NewGuid(),
                SenderId = model.SenderId,
                Content = model.Content,
                Attachments = model.Attachments,
                IsPinned = model.IsPinned,
                ParentId = model.ParentId,
                Type = MessageType.Room,
                RoomId = roomId,
                CreateTime = DateTime.Now.Ticks,
            };
            await BroadcastEvent(roomId, ReceiveMessageRoom, result);
            ValidateMessage(roomId, null, model);
            var domain = _mapper.Map<Message>(result);
            await _messageService.AddAsync(domain);
        }, "Failed to send message");
    }

    public async Task DeleteMessage(Guid roomId, Guid messageId)
    {
        await ExecuteWithErrorHandling(async () =>
        {
            await BroadcastEvent(roomId, DeleteMessageRoom, messageId);
            ValidateMessage(roomId, messageId);
            await _messageService.DeleteByIdAsync(messageId);
        }, "Failed to delete message");
    }

    public async Task UpdateMessage(Guid roomId, Guid messageId, MessageCreationModel model)
    {
        await ExecuteWithErrorHandling(async () =>
        {
            var result = new MessageModel
            {
                Id = messageId,
                SenderId = model.SenderId,
                Content = model.Content,
                Attachments = model.Attachments,
                IsPinned = model.IsPinned,
                ParentId = model.ParentId,
                Type = MessageType.Room,
                RoomId = roomId,
                IsEdited = true,
                LastModifyTime = DateTime.Now.Ticks,
            };
            await BroadcastEvent(roomId, UpdateMessageRoom, result);
            ValidateMessage(roomId, messageId, model);
            var domain = _mapper.Map<Message>(result);
            await _messageService.UpdateAsync(messageId, domain);
        }, "Failed to update message");
    }

    public async Task SendMessageReaction(Guid roomId, MessageReactionCreationModel model)
    {
        ValidateMessage(roomId, null, model);
        await ExecuteWithErrorHandling(async () =>
        {
            var domain = _mapper.Map<MessageReaction>(model);
            domain.InitCreation();
            var result = _mapper.Map<MessageReactionModel>(domain);
            await BroadcastEvent(roomId, ReceiveMessageReactionRoom, result);
            await _messageReactionService.AddAsync(domain);
        }, "Failed to send message reaction");
    }

    public async Task DeleteMessageReaction(Guid roomId, Guid reactionId)
    {
        ValidateMessage(roomId, reactionId);
        await ExecuteWithErrorHandling(async () =>
        {
            await BroadcastEvent(roomId, DeleteMessageReactionRoom, reactionId);
            await _messageReactionService.DeleteByIdAsync(reactionId);
        }, "Failed to delete message reaction");
    }

    public async Task UpdateMessageReaction(Guid roomId, Guid reactionId, MessageReactionCreationModel model)
    {
        ValidateMessage(roomId, reactionId, model);
        await ExecuteWithErrorHandling(async () =>
        {
            var domain = _mapper.Map<MessageReaction>(model);
            domain.Id = reactionId;
            domain.UpdateModify();
            var result = _mapper.Map<MessageReactionModel>(domain);
            await BroadcastEvent(roomId, UpdateMessageReactionRoom, result);
            await _messageReactionService.UpdateAsync(reactionId, domain);
        }, "Failed to update message reaction");
    }

    private async Task BroadcastEvent(Guid roomId, string eventName, object data)
    {
        await Clients.Group(roomId.ToString()).SendAsync(eventName, data);
    }
    private async Task ExecuteWithErrorHandling(Func<Task> action, string errorMessage)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"{errorMessage}: {ex.Message}");
            throw;
        }
    }
    private static void ValidateMessage(Guid roomId, Guid? targetId = null, object? model = null)
    {
        if (roomId == Guid.Empty ||
            (targetId.HasValue && targetId.Value == Guid.Empty) ||
            (model != null && model == null))
        {
            throw new ParameterInvalidException();
        }
    }

}