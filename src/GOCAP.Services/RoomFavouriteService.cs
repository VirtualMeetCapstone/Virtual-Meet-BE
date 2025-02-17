namespace GOCAP.Services;

[RegisterService(typeof(IRoomFavouriteService))]
internal class RoomFavouriteService(
    IRoomFavouriteRepository _repository,
    IUserRepository _userRepository,
    IRoomRepository _roomRepository,
    IMapper _mapper,
    ILogger<RoomFavouriteService> _logger
    ) : ServiceBase<RoomFavourite, RoomFavouriteEntity>(_repository, _mapper, _logger), IRoomFavouriteService
{
    private readonly IMapper _mapper = _mapper;
    /// <summary>
    /// Create or delete the room favourite.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public async Task<OperationResult> CreateOrDeleteAsync(RoomFavourite domain)
    {
        if (!await _userRepository.CheckExistAsync(domain.UserId))
        {
            throw new ResourceNotFoundException($"User {domain.UserId} was not found.");
        }

        if (!await _roomRepository.CheckExistAsync(domain.RoomId))
        {
            throw new ResourceNotFoundException($"User {domain.RoomId} was not found.");
        }

        var result = new OperationResult(true);
        var roomFavourite = await _repository.GetByRoomAndUserAsync(domain.RoomId, domain.UserId);
        if (roomFavourite != null)
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(RoomFavourite).Name);
            var resultDelete = await _repository.DeleteByIdAsync(roomFavourite.Id);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(RoomFavourite).Name);
            domain.InitCreation();
            var entity = _mapper.Map<RoomFavouriteEntity>(domain);
            await _repository.AddAsync(entity);
        }
        return result;
    }
}
