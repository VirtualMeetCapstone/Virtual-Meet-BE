namespace GOCAP.Services.Intention;

public interface IRoomService : IServiceBase<Room>
{
    Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomCount> GetRoomCountsAsync();
}