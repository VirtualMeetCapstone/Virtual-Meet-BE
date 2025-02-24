
namespace GOCAP.Services.Intention;

public interface ISearchHistoryService : IServiceBase<SearchHistory>
{
    Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit);
}
