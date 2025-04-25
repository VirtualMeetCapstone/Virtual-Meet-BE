namespace GOCAP.Services;

[RegisterService(typeof(IRoomService))]
internal class RoomService(
    IRoomRepository _repository,
    IRoomMemberRepository _roomMemberRepository,
    IBlobStorageService _blobStorageService,
    IUserContextService _userContextService,
    IUnitOfWork _unitOfWork,
    IKafkaProducer _kafkaProducer,
    IRedisService _redisService,
    IMapper _mapper,
    ILogger<RoomService> _logger
    ) : ServiceBase<Room, RoomEntity>(_repository, _mapper, _logger), IRoomService
{
    private readonly IMapper _mapper = _mapper;
    private readonly string _redisKey = "Room:Password:";
    public override async Task<Room> AddAsync(Room room)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Room).Name);

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
        entity.OwnerId = _userContextService.Id;

        if (!string.IsNullOrEmpty(room.Password))
        {
            _logger.LogInformation("Storing password for Room ID: {RoomId} in Redis.", room.Id);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(room.Password);
            var redisKey = $"{_redisKey}{room.Id}";
            var isCreatedPassword = await _redisService.SetAsync(redisKey, passwordHash);
            if (!isCreatedPassword)
            {
                throw new ParameterInvalidException("Failed to store room password.");
            }
        }

        try
        {
            var result = await _repository.AddAsync(entity);

            _ = _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, new NotificationEvent
            {
                Type = NotificationType.Room,
                ActionType = ActionType.Add,
                Source = new NotificationSource { Id = result.Id },
                ActorId = result.OwnerId
            });

            return _mapper.Map<Room>(result);
        }
        catch (Exception ex)
        {
            var redisKey = $"{_redisKey}{room.Id}";
            _logger.LogWarning(ex, "Room creation failed. Deleting password in Redis for Room ID: {RoomId}", room.Id);
            await _redisService.DeleteAsync(redisKey);
            throw;
        }
    }

    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Room).Name);
        var room = await _repository.GetByIdAsync(id);
        if (room.OwnerId != _userContextService.Id && !_userContextService.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException("You are not the owner of this room.");
        }

        var medias = JsonHelper.Deserialize<List<Media>>(room.Medias);
        if (medias != null && medias.Count > 0)
        {
            var isFileDeleted = await _blobStorageService.DeleteFilesByUrlsAsync([.. medias.Select(x => x.Url)]);
            if (!isFileDeleted)
            {
                return new OperationResult(isFileDeleted, "Unexpected error occurs while deleting media file.");
            }
        }
        try
        {
            // Delete password in redis cache.
            _logger.LogInformation("Delete password for Room ID: {RoomId} in Redis.", room.Id);
            var redisKey = $"{_redisKey}{id}";
            await _redisService.DeleteAsync(redisKey);

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
        if (entity.OwnerId != _userContextService.Id && !_userContextService.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException("You are not the owner of this room.");
        }
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
        if (!string.IsNullOrEmpty(domain.Password))
        {
            _logger.LogInformation("Update password for Room Id: {RoomId} in Redis.", domain.Id);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(domain.Password);
            var redisKey = $"{_redisKey}{id}";
            await _redisService.DeleteAsync(redisKey);
            await _redisService.SetAsync(redisKey, passwordHash);
        }

        entity.Privacy = domain.Privacy;
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
    {
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            _ = _kafkaProducer.ProduceAsync(KafkaConstants.Topics.SearchHistory, new SearchHistory
            {
                Query = queryInfo.SearchText,
                UserId = _userContextService.Id,
            });
        }
        return await _repository.GetWithPagingAsync(queryInfo);
    }

    public async Task<Room> GetDetailByIdAsync(Guid id)
    => await _repository.GetDetailByIdAsync(id) ?? throw new ResourceNotFoundException($"Room {id} was not found.");
}