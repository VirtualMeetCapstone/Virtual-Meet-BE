using GOCAP.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GOCAP.Api.Controllers;

public class MessagesController(IMessageService _service, IMapper _mapper, IHubContext<ChatHub> _chatHub)
{
    [HttpPost("room-messages")]
    public async Task<RoomMessageModel> SendMessage([FromBody] RoomMessageModel model)
    {
        var domain = _mapper.Map<RoomMessage>(model);
        await _service.AddRoomMessageAsync(domain);
        await _chatHub.Clients.Groups(model.Id.ToString()).SendAsync(model.Content);

        return model;
    }
}
