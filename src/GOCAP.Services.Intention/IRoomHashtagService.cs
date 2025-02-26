namespace GOCAP.Services.Intention;

public interface IRoomHashtagService : IServiceBase<RoomHashtag>
{
	Task<QueryResult<Room>> GetRoomByHashTagWithPagingAsync(string tag, QueryInfo queryInfo);
}
