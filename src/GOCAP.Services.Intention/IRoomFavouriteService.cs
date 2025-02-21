namespace GOCAP.Services.Intention;

public interface IRoomFavouriteService : IServiceBase<RoomFavourite>
{
    Task<OperationResult> CreateOrDeleteAsync(RoomFavourite domain);
	Task<QueryResult<Room>> GetRoomFavouriteAsync(Guid userId, QueryInfo queryInfo);

}

