
namespace GOCAP.Services;

[RegisterService(typeof(ISearchHistoryService))]
internal class SearchHistoryService(
    ISearchHistoryRepository _repository,
    IMapper _mapper,
    ILogger<SearchHistoryService> _logger
    ) : ServiceBase<SearchHistory, SearchHistoryEntity>(_repository, _mapper, _logger), ISearchHistoryService
{
    public async Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit)
    => await _repository.GetPopularSearchSuggestionsAsync(prefix, limit);
}
