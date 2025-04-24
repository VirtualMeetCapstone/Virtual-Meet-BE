namespace GOCAP.Services;
[RegisterService(typeof(IRoomStatisticService))]
internal class RoomStatisticService(IRoomStatisticsRepository _repository) : IRoomStatisticService
{
    public async Task<IEnumerable<RoomStatisticsEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _repository.GetByDateRangeAsync(startDate, endDate);
    }

    public async Task<RoomStatisticsEntity> GetByRoomIdAsync(string roomId)
    {
        return await _repository.GetByRoomIdAsync(roomId);
    }

    public async Task<QueryResult<RoomStatisticsEntity>> GetWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetWithPagingAsync(queryInfo);
    }
}
