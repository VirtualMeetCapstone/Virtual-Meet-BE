using GOCAP.Database;

namespace GOCAP.Services.Intention;

public interface IRoomStatisticService
{
    Task<QueryResult<RoomStatisticsEntity>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomStatisticsEntity> GetByRoomIdAsync(string roomId);
    Task<IEnumerable<RoomStatisticsEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}
