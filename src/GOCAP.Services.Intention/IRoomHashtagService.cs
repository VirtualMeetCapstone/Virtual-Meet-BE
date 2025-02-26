namespace GOCAP.Services.Intention;

public interface IRoomHashTagService : IServiceBase<RoomHashTag>
{
	Task<QueryResult<Room>> GetRoomByHashTagWithPagingAsync(string tag, QueryInfo queryInfo);
}
