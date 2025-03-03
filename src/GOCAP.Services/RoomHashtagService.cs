namespace GOCAP.Services;

[RegisterService(typeof(IRoomHashTagService))]
internal class RoomHashTagService(
	IRoomHashTagRepository _repository,
	IMapper _mapper,
    ILogger<RoomHashTagService> _logger
    ) : ServiceBase<RoomHashTag, RoomHashTagEntity>(_repository, _mapper, _logger), IRoomHashTagService
{
	public async Task<QueryResult<Room>> GetRoomsByHashTagWithPagingAsync(string tag, QueryInfo queryInfo)
		=> await _repository.GetRoomsByHashTagWithPagingAsync(tag, queryInfo);
}
