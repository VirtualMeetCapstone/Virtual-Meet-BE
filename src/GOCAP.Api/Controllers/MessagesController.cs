using GOCAP.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Controllers;

public class MessagesController(IMessageService _service, IMapper _mapper, IHubContext<ChatHub> _chatHub)
{
    [HttpPost("messages")]
    [ValidateModel]
    public async Task<MessageModel> SendMessage([FromBody] MessageCreationModel model)
    {
        var domain = _mapper.Map<Message>(model);
        var result = await _service.AddAsync(domain);
        await _chatHub.Clients.Groups(domain.Id.ToString()).SendAsync(model.Content);
        return _mapper.Map<MessageModel>(result);
    }

}