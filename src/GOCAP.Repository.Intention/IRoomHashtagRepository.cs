namespace GOCAP.Repository.Intention;

public interface IRoomHashTagRepository : ISqlRepositoryBase<RoomHashTagEntity>
{
	Task<QueryResult<Room>> GetRoomsByHashTagsWithPagingAsync(string tag, QueryInfo queryInfo);
}
