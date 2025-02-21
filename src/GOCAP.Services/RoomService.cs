namespace GOCAP.Services;

[RegisterService(typeof(IRoomService))]
internal class RoomService(
    IRoomRepository _repository,
    IRoomMemberRepository _roomMemberRepository,
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IBlobStorageService _blobStorageService,
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

        // Upload file to azure.
        if (room.MediaUploads?.Count > 0)
        {
            var medias = await _blobStorageService.UploadFilesAsync(room.MediaUploads);
            room.Medias = medias;
        }

        room.InitCreation();
        try
        {
            var entity = _mapper.Map<RoomEntity>(room);
            var result = await _repository.AddAsync(entity);
            return _mapper.Map<Room>(result);
        }
        catch (Exception ex)
        {
            if (room.MediaUploads != null && room.MediaUploads.Count > 0)
            {
                await MediaHelper.DeleteMediaFilesIfError(room.MediaUploads, _blobStorageService);
            }
            throw new InternalException(ex.Message);
        }
    }

    /// <summary>
    /// Delete a room by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Room).Name);
        try
        {
            // Begin transaction by unit of work to make sure the consistency
            await _unitOfWork.BeginTransactionAsync();

            await _roomMemberRepository.DeleteByRoomIdAsync(id);
            var result = await _repository.DeleteByIdAsync(id);

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
        if (domain.MediaUploads?.Count > 0)
        {
            var medias = await _blobStorageService.UploadFilesAsync(domain.MediaUploads);
            domain.Medias = medias;
        }
        domain.UpdateModify();
        try
        {
            var entity = _mapper.Map<RoomEntity>(domain);
            return new OperationResult(await _repository.UpdateAsync(entity));
        }
        catch (Exception ex)
        {
            if (domain.MediaUploads != null && domain.MediaUploads.Count > 0)
            {
                await MediaHelper.DeleteMediaFilesIfError(domain.MediaUploads, _blobStorageService);
            }
            
            throw new InternalException(ex.Message);
        }
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