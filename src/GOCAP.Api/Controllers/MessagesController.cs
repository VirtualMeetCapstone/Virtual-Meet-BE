using GOCAP.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Controllers;

[Route("messages")]
public class MessagesController(IHubContext<ChatHub> _chatHub)
{
    [HttpPost("send")]
    public async Task<MessageModel> SendMessage([FromBody] MessageModel message)
    {
        await _chatHub.Clients.Groups(message.Id.ToString()).SendAsync(message.Content);

        return message;
    }
}
