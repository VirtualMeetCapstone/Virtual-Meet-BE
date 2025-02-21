namespace GOCAP.Repository.Intention;

public interface IRoomFavouriteRepository : ISqlRepositoryBase<RoomFavouriteEntity>
{
    Task<RoomFavouriteEntity?> GetByRoomAndUserAsync(Guid roomId, Guid userId);
	Task<QueryResult<Room>> GetRoomFavouriteAsync(Guid userId, QueryInfo queryInfo);

}
