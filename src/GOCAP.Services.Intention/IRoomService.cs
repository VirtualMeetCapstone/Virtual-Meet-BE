﻿namespace GOCAP.Services.Intention;

public interface IRoomService : IServiceBase<Room>
{
    Task<Room> GetDetailByIdAsync(Guid id);
    Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<RoomCount> GetRoomCountsAsync();
}