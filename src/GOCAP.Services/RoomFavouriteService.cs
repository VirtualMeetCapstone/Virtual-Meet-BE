namespace GOCAP.Services;

[RegisterService(typeof(IRoomFavouriteService))]
internal class RoomFavouriteService(
    IRoomFavouriteRepository _repository,
    ILogger<RoomFavouriteService> _logger
    ) : ServiceBase<RoomFavourite>(_repository, _logger), IRoomFavouriteService
{
}
