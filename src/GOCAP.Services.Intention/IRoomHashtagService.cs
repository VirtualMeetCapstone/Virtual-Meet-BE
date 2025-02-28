namespace GOCAP.Services.Intention;

public interface IRoomHashTagService : IServiceBase<RoomHashTag>
{
	Task<QueryResult<Room>> GetRoomsByHashTagWithPagingAsync(string tag, QueryInfo queryInfo);
}
