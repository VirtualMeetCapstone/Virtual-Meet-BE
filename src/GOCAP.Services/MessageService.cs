namespace GOCAP.Services;

[RegisterService(typeof(IMessageService))]
internal class MessageService(
    IMessageRepository _repository,
    IUserRepository _userRepository,
    IRoomRepository _roomRepository,
    IGroupRepository _groupRepository,
    IBlobStorageService _blobStorageService,
    IUserContextService _userContextService,
    IMapper _mapper,
    ILogger<MessageService> _logger
    ) : ServiceBase<Message, MessageEntity>(_repository, _mapper, _logger), IMessageService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<Message> AddAsync(Message domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Message).Name);
        await ValidateMessage(domain);
        var entity = _mapper.Map<MessageEntity>(domain);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<Message>(result);
    }

    public override async Task<OperationResult> UpdateAsync(Guid id, Message domain)
    {
        _logger.LogInformation("Start updating entity of type {EntityType}.", typeof(Message).Name);
        var isMessageExists = await _repository.CheckExistAsync(id);
        if (!isMessageExists)
        {
            throw new ParameterInvalidException($"Message {id} does not exist.");
        }
        await ValidateMessage(domain);
        var entity = _mapper.Map<MessageEntity>(domain);
        return new OperationResult(await _repository.UpdateAsync(entity));
    }

    public async Task<QueryResult<Conversation>> GetConversations(QueryInfo queryInfo)
    {
        //return await _repository.GetConversations(_userContextService.Id, queryInfo); thay error cai nay
        return await _repository.GetMessagesByCurrentUserId(_userContextService.Id, queryInfo);
    }

    private async Task ValidateMessage(Message domain)
    {
        if (domain.Attachments != null && domain.Attachments.Count > 0)
        {
            var urls = domain.Attachments.Select(x => x.Url).ToList();
            var isExists = await _blobStorageService.CheckFilesExistByUrlsAsync(urls);
            if (!isExists)
            {
                throw new ParameterInvalidException("At least one media file uploaded is invalid.");
            }
            domain.Attachments.ToList().ForEach(x => x.Type = ConvertMediaHelper.GetMediaTypeFromUrl(x.Url));
        }
        switch (domain.Type)
        {
            case MessageType.Direct:
                var isUserExists = await _userRepository.CheckExistAsync(domain.ReceiverId ?? Guid.Empty);
                if (!isUserExists)
                {
                    throw new ParameterInvalidException($"User {domain.ReceiverId} does not exists.");
                }
                break;
            case MessageType.Room:
                var isRoomExists = await _roomRepository.CheckExistAsync(domain.RoomId ?? Guid.Empty);
                if (!isRoomExists)
                {
                    throw new ParameterInvalidException($"Room {domain.RoomId} does not exists.");
                }
                break;
            case MessageType.Group:
                var isGroupExists = await _groupRepository.CheckExistAsync(domain.GroupId ?? Guid.Empty);
                if (!isGroupExists)
                {
                    throw new ParameterInvalidException($"Group {domain.GroupId} does not exists.");
                }
                break;
        }
    }
}
