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
    /// <param name="roomFavourite"></param>
    /// <returns></returns>
    public async Task<OperationResult> CreateOrDeleteAsync(RoomFavourite roomFavourite)
    {
        var result = new OperationResult(true);
        var isExists = await _repository.CheckExistAsync(roomFavourite.RoomId, roomFavourite.UserId);
        if (isExists)
        {
            var resultDelete = await _repository.DeleteAsync(roomFavourite.RoomId, roomFavourite.UserId);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(RoomFavourite).Name);
            roomFavourite.Id = Guid.NewGuid();
            roomFavourite.CreateTime = DateTime.UtcNow.Ticks;
            await _repository.AddAsync(roomFavourite);
        }
        return result;
    }
}
