using GOCAP.Messaging.Producer;

namespace GOCAP.Services;

[RegisterService(typeof(IRoomService))]
internal class RoomService(
    IRoomRepository _repository,
    IRoomMemberRepository _roomMemberRepository,
    IUserRepository _userRepository,
    IBlobStorageService _blobStorageService,
    IUnitOfWork _unitOfWork,
    IKafkaProducer _kafkaProducer,
    IMapper _mapper,
    ILogger<RoomService> _logger
    ) : ServiceBase<Room, RoomEntity>(_repository, _mapper, _logger), IRoomService
{
    private readonly IMapper _mapper = _mapper;
    /// <summary>
    /// Create a new roomMd
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public override async Task<Room> AddAsync(Room room)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Room).Name);

        if (!await _userRepository.CheckExistAsync(room.OwnerId))
        {
            throw new ResourceNotFoundException($"User {room.OwnerId} was not found.");
        }
        if (room.Medias != null && room.Medias.Count > 0)
        {
            var urls = room.Medias.Select(x => x.Url).ToList();
            var isExists = await _blobStorageService.CheckFilesExistByUrlsAsync(urls);
            if (!isExists)
            {
                throw new ParameterInvalidException("At least one media file uploaded is invalid.");
            }
            room.Medias.ToList().ForEach(x => x.Type = ConvertMediaHelper.GetMediaTypeFromUrl(x.Url));
        }
        room.InitCreation();
        var entity = _mapper.Map<RoomEntity>(room);
        var result = await _repository.AddAsync(entity);
        await _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, new NotificationEvent
        {
            Type = NotificationType.Room,
            ActionType = ActionType.Add,
            Source = new NotificationSource
            {
                Id = result.Id
            },
            ActorId = result.OwnerId
        });
        return _mapper.Map<Room>(result);
    }

    /// <summary>
    /// Delete a room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Room).Name);
        var room = await _repository.GetByIdAsync(id);
        var medias = JsonHelper.Deserialize<List<Media>>(room.Medias);
        if (medias != null && medias.Count > 0)
        {
            var isFileDeleted = await _blobStorageService.DeleteFilesByUrlsAsync(medias.Select(x => x.Url).ToList());
            if (!isFileDeleted)
            {
                return new OperationResult(isFileDeleted, "Unexpected error occurs while deleting media file.");
            }
        }
        try
        {
            // Begin transaction by unit of work to make sure the consistency
            await _unitOfWork.BeginTransactionAsync();

            await _roomMemberRepository.DeleteByRoomIdAsync(id);
            var result = await _repository.DeleteByEntityAsync(room);

            // Commit if success
            await _unitOfWork.CommitTransactionAsync();
            return new OperationResult(result > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(Room).Name);
            // Rollback if fail
            await _unitOfWork.RollbackTransactionAsync();
            return new OperationResult(false);
        }
    }

    public override async Task<OperationResult> UpdateAsync(Guid id, Room domain)
    {
        _logger.LogInformation("Start updating entity of type {EntityType}.", typeof(Room).Name);
        domain.UpdateModify();
        var entity = await _repository.GetByIdAsync(domain.Id, false);
        if (domain.Medias != null && domain.Medias.Count > 0)
        {
            if (!string.IsNullOrEmpty(entity.Medias))
            {
                var medias = JsonHelper.Deserialize<List<Media>>(entity.Medias);
                if (medias != null && medias.Count > 0)
                {
                    var urls = medias.Select(m => m.Url).ToList();
                    await _blobStorageService.DeleteFilesByUrlsAsync(urls);
                }
            }
            entity.Medias = JsonHelper.Serialize(domain.Medias);
        }
        
        entity.Topic = domain.Topic;
        entity.Description = domain.Description;
        entity.MaximumMembers = domain.MaximumMembers;
        entity.LastModifyTime = domain.LastModifyTime;
        
        return new OperationResult(await _repository.UpdateAsync(entity));
    }

    public async Task<RoomCount> GetRoomCountsAsync()
    {
        return await _repository.GetRoomCountsAsync();
    }

    public async Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo)
    => await _repository.GetWithPagingAsync(queryInfo);

    public async Task<Room> GetDetailIdAsync(Guid id)
    => await _repository.GetDetailIdAsync(id);
}