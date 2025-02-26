namespace GOCAP.Services;

[RegisterService(typeof(IRoomHashTagService))]
internal class RoomHashTagService(
	IRoomHashTagRepository _repository,
	IMapper _mapper,
    ILogger<RoomHashTagService> _logger
    ) : ServiceBase<RoomHashTag, RoomHashTagEntity>(_repository, _mapper, _logger), IRoomHashTagService
{
	private readonly IMapper _mapper = _mapper;

	public async Task<QueryResult<Room>> GetRoomByHashTagWithPagingAsync(string tag, QueryInfo queryInfo)
		=> await _repository.GetRoomByHashtagsWithPagingAsync(tag, queryInfo);
}
