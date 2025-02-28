namespace GOCAP.Services;

[RegisterService(typeof(IMessageService))]
internal class MessageService(
    IMessageRepository _repository,
    IMapper _mapper,
    ILogger<MessageService> _logger
    ) : ServiceBase<Message, MessageEntity>(_repository, _mapper, _logger), IMessageService
{
    private readonly IMapper _mapper = _mapper;
    public async Task<RoomMessage> AddRoomMessageAsync(RoomMessage domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(RoomMessage).Name);
        domain.InitCreation();
        var entity = _mapper.Map<RoomMessageEntity>(domain);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<RoomMessage>(result);
    }
}
