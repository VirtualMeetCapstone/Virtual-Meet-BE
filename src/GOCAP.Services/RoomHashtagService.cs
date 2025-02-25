namespace GOCAP.Services;

[RegisterService(typeof(IRoomHashtagService))]
internal class RoomHashtagService(
	IRoomHashtagRepository _repository,
	IMapper _mapper,
    ILogger<RoomHashtagService> _logger
    ) : ServiceBase<RoomHashtag, RoomHashTagEntity>(_repository, _mapper, _logger), IRoomHashtagService
{
	private readonly IMapper _mapper = _mapper;

	public async Task<QueryResult<Room>> GetRoomHashTagWithPagingAsync(string tag, QueryInfo queryInfo)
		=> await _repository.GetRoomHashtagsWithPagingAsync(tag, queryInfo);
}
