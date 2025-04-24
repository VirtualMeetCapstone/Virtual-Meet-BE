namespace GOCAP.Repository;

public interface IRoomStatisticsRepository : IMongoRepositoryBase<RoomStatisticsEntity>
{
    Task<QueryResult<RoomStatisticsEntity>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomStatisticsEntity> GetByRoomIdAsync(string roomId);
    Task<IEnumerable<RoomStatisticsEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}
