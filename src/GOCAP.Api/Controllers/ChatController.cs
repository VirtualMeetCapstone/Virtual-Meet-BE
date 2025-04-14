namespace GOCAP.Api.Controllers;

public class ChatController (IMessageService _service, IMapper _mapper) : ApiControllerBase
{
    [Authorize]
    [HttpGet("chat/conversations")]
    public async Task<QueryResult<ConversationModel>> GetConversationsByCurrentUser([FromQuery] QueryInfo queryInfo)
    {
        var result = await _service.GetConversations(queryInfo);
        return _mapper.Map<QueryResult<ConversationModel>>(result);
    }
}