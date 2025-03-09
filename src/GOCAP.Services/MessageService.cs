namespace GOCAP.Services;

[RegisterService(typeof(IMessageService))]
internal class MessageService(
    IMessageRepository _repository,
    IMapper _mapper,
    ILogger<MessageService> _logger
    ) : ServiceBase<Message, MessageEntity>(_repository, _mapper, _logger), IMessageService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<Message> AddAsync(Message domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Message).Name);
        switch (domain.Type)
        {
            case MessageType.Direct:
                break;
            case MessageType.Room:
                break;
            case MessageType.Group:
                break;
        }
        domain.InitCreation();
        var entity = _mapper.Map<MessageEntity>(domain);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<Message>(result);
    }
}
