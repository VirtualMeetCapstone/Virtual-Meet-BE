
namespace GOCAP.Services;

[RegisterService(typeof(ISearchHistoryService))]
internal class SearchHistoryService(
    ISearchHistoryRepository _repository,
    IMapper _mapper,
    ILogger<SearchHistoryService> _logger
    ) : ServiceBase<SearchHistory, SearchHistoryEntity>(_repository, _mapper, _logger), ISearchHistoryService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<SearchHistory> AddAsync(SearchHistory domain)
    {
        _logger.LogInformation("Adding a new search history entry.");

        domain.Query = domain.Query.Trim();

        if (await _repository.GetByQueryAsync(domain.Query) is { } existingEntity)
        {
            _logger.LogInformation("Query '{Query}' already exists for user {UserId}. Skipping insert.", domain.Query, domain.UserId);
            return _mapper.Map<SearchHistory>(existingEntity);
        }

        domain.InitCreation();
        var entity = _mapper.Map<SearchHistoryEntity>(domain);
        var result = await _repository.AddAsync(entity);

        return _mapper.Map<SearchHistory>(result);
    }

    public async Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit)
    => await _repository.GetPopularSearchSuggestionsAsync(prefix, limit);
}
