
namespace GOCAP.Repository.Intention;

public interface ISearchHistoryRepository : IMongoRepositoryBase<SearchHistoryEntity>
{
    Task<SearchHistoryEntity> GetByQueryAsync(string query);
    Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit);
}
