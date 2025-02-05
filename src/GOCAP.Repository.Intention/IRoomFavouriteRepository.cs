namespace GOCAP.Repository.Intention;

public interface IRoomFavouriteRepository : ISqlRepositoryBase<RoomFavourite>
{
    Task<RoomFavourite?> GetByRoomAndUserAsync(Guid roomId, Guid userId);
}
