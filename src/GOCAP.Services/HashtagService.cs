namespace GOCAP.Services;

[RegisterService(typeof(IHashtagService))]
internal class HashtagService(
	IHashtagRepository _repository,
	IMapper _mapper,
	ILogger<HashtagService> _logger
	) : ServiceBase<Hashtag, HashTagEntity>(_repository, _mapper, _logger), IHashtagService
{
	private readonly IMapper _mapper = _mapper;

	public Task<List<Hashtag>> SearchHashtagsAsync(string prefix, int limit)
	=> _repository.SearchHashtagsAsync(prefix, limit);
}
