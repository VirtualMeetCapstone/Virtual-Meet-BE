namespace GOCAP.Services;

[RegisterService(typeof(IMessageService))]
internal class MessageService(
    IMessageRepository _repository,
    IMapper _mapper,
    ILogger<MessageService> _logger
    ) : ServiceBase<Message, MessageEntity>(_repository, _mapper, _logger), IMessageService
{
}
