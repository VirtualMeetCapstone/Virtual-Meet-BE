namespace GOCAP.Repository.Intention;

public interface IRoomRepository : ISqlRepositoryBase<RoomEntity>
{
    Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomCount> GetRoomCountsAsync();
}
