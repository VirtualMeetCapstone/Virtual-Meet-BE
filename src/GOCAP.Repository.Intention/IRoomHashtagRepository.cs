namespace GOCAP.Repository.Intention;

public interface IRoomHashTagRepository : ISqlRepositoryBase<RoomHashTagEntity>
{
	Task<QueryResult<Room>> GetRoomsByHashTagWithPagingAsync(string tag, QueryInfo queryInfo);
}
