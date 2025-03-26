
namespace GOCAP.Services.Intention;

public interface ISearchHistoryService : IServiceBase<SearchHistory>
{
    Task<List<string>> GetSearchByUserIdAsync(Guid userId, int limit);
    Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit);
}
