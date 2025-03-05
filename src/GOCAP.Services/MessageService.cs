namespace GOCAP.Services;

[RegisterService(typeof(IMessageService))]
internal class MessageService(
    IMessageRepository<MessageEntity> _repository,
    IMessageRepositoryFactory _messageRepositoryFactory,
    IMapper _mapper,
    ILogger<MessageService> _logger
    ) : ServiceBase<Message, MessageEntity>(_repository, _mapper, _logger), IMessageService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<Message> AddAsync(Message domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Message).Name);
        domain.InitCreation();
        var repository = _messageRepositoryFactory.Create(domain.Type) 
            ?? throw new InternalException();
        var result = new Message();
        switch (domain.Type)
        {
            case MessageType.Direct:
                var userEntity = _mapper.Map<UserMessageEntity>(domain);
                var entity = await repository.AddAsync(userEntity);
                result = _mapper.Map<Message>(entity);
                break;
            case MessageType.Room:
                break;
            case MessageType.Group:
                break;
        }
        return _mapper.Map<Message>(result);
    }
}
