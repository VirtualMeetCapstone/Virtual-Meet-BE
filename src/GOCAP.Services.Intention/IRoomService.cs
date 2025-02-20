namespace GOCAP.Services.Intention;

public interface IRoomService : IServiceBase<Room>
{
    Task<Room> GetDetailIdAsync(Guid id);
    Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomCount> GetRoomCountsAsync();
}