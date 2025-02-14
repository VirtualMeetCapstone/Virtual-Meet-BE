namespace GOCAP.Services;

[RegisterService(typeof(IRoomService))]
internal class RoomService(
    IRoomRepository _repository,
    IRoomMemberRepository _roomMemberRepository,
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IBlobStorageService _blobStorageService,
    ILogger<RoomService> _logger
    ) : ServiceBase<Room>(_repository, _logger), IRoomService
{
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
            var medias = await _blobStorageService.UploadFileAsync(room.MediaUploads);
            room.Medias = medias;
        }

        room.InitCreation();
        try
        {
            return await _repository.AddAsync(room);
        }
        catch (Exception ex)
        {
            await DeleteMediaFileIfError(room);
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
            var medias = await _blobStorageService.UploadFileAsync(domain.MediaUploads);
            domain.Medias = medias;
        }
        domain.UpdateModify();
        try
        {
            return new OperationResult(await _repository.UpdateAsync(id, domain));
        }
        catch (Exception ex)
        {
            await DeleteMediaFileIfError(domain);
            throw new InternalException(ex.Message);
        }
    }

    private async Task DeleteMediaFileIfError(Room domain)
    {
        var mediaDeletes = new List<MediaDelete>();
        var mediaDeleteDict = new Dictionary<string, MediaDelete>();

        if (domain.MediaUploads is not null)
        {
            foreach (var mediaUpload in domain.MediaUploads)
            {
                if (!mediaDeleteDict.TryGetValue(mediaUpload.ContainerName, out var mediaDelete))
                {
                    mediaDelete = new MediaDelete
                    {
                        ContainerName = mediaUpload.ContainerName,
                        FileNames = []
                    };
                    mediaDeleteDict[mediaUpload.ContainerName] = mediaDelete;
                    mediaDeletes.Add(mediaDelete);
                }
                mediaDelete.FileNames.Add(mediaUpload.FileName);
            }
        }
        await _blobStorageService.DeleteFilesAsync(mediaDeletes);
    }

    public async Task<RoomCount> GetRoomCountsAsync()
    {
        return await _repository.GetRoomCountsAsync();
    }
}