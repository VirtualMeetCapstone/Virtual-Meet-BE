namespace GOCAP.Repository.Intention;

public interface IRoomRepository : ISqlRepositoryBase<RoomEntity>
{
    Task<Room> GetDetailByIdAsync(Guid id);
    Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomCount> GetRoomCountsAsync();
}
