﻿namespace GOCAP.Services;

[RegisterService(typeof(IHashTagService))]
internal class HashTagService(
	IHashTagRepository _repository,
	IMapper _mapper,
	ILogger<HashTagService> _logger
	) : ServiceBase<HashTag, HashTagEntity>(_repository, _mapper, _logger), IHashTagService
{
	public Task<List<HashTag>> SearchHashTagsAsync(string prefix, int limit)
	=> _repository.SearchHashTagsAsync(prefix, limit);
}
