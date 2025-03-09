using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Controllers;

public class MessagesController(IMessageService _service,
    IMapper _mapper, 
    IHubContext<ChatHub> _chatHub)
{
    [HttpPost("messages")]
    [ValidateModel]
    public async Task<MessageModel> SendMessage([FromBody] MessageCreationModel model)
    {
        var domain = _mapper.Map<Message>(model);
        var result = await _service.AddAsync(domain);
        switch (model.Type)
        {
            case MessageType.Direct:
                await _chatHub.Clients.Groups(model.ReceiverId.ToString() ?? "").SendAsync(model.Content);
                break;
            case MessageType.Room:
                await _chatHub.Clients.Groups(model.RoomId.ToString() ?? "").SendAsync(model.Content);
                break;
            case MessageType.Group:
                await _chatHub.Clients.Groups(model.GroupId.ToString() ?? "").SendAsync(model.Content);
                break;
        }
        return _mapper.Map<MessageModel>(result);
    }

}