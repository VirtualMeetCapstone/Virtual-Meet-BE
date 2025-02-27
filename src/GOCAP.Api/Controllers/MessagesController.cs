using GOCAP.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Controllers;

[Route("messages")]
public class MessagesController(IMessageService _service, IMapper _mapper, IHubContext<ChatHub> _chatHub)
{
    [HttpPost("send")]
    public async Task<MessageModel> SendMessage([FromBody] MessageModel model)
    {
        var domain = _mapper.Map<Message>(model);
        await _service.AddAsync(domain);
        await _chatHub.Clients.Groups(model.Id.ToString()).SendAsync(model.Content);

        return model;
    }
}
