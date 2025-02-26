namespace GOCAP.Repository.Intention;

public interface IRoomHashTagRepository : ISqlRepositoryBase<RoomHashTagEntity>
{
	Task<QueryResult<Room>> GetRoomByHashtagsWithPagingAsync(string tag, QueryInfo queryInfo);
}
