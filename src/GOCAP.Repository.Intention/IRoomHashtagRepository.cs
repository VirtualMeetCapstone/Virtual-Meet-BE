namespace GOCAP.Repository.Intention;

public interface IRoomHashtagRepository : ISqlRepositoryBase<RoomHashTagEntity>
{
	Task<QueryResult<Room>> GetRoomHashtagsWithPagingAsync(string tag, QueryInfo queryInfo);
}
