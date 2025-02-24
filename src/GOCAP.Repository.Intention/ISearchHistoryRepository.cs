
namespace GOCAP.Repository.Intention;

public interface ISearchHistoryRepository : IMongoRepositoryBase<SearchHistoryEntity>
{
    Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit);
}
