namespace GOCAP.Services.Intention;

public interface IRoomHashtagService : IServiceBase<RoomHashtag>
{
	Task<QueryResult<Room>> GetRoomHashTagWithPagingAsync(string tag, QueryInfo queryInfo);
}
