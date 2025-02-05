namespace GOCAP.Services;

[RegisterService(typeof(IRoomFavouriteService))]
internal class RoomFavouriteService(
    IRoomFavouriteRepository _repository,
    ILogger<RoomFavouriteService> _logger
    ) : ServiceBase<RoomFavourite>(_repository, _logger), IRoomFavouriteService
{
    /// <summary>
    /// Create or delete the room favourite.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public async Task<OperationResult> CreateOrDeleteAsync(RoomFavourite domain)
    {
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
            await _repository.AddAsync(domain);
        }
        return result;
    }
}
