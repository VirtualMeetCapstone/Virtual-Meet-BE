namespace GOCAP.Repository.Intention;

public interface IRoomHashtagRepository : ISqlRepositoryBase<RoomHashTagEntity>
{
	Task<QueryResult<Room>> GetRoomByHashtagsWithPagingAsync(string tag, QueryInfo queryInfo);
}
