
namespace GOCAP.Repository.Intention;

public interface ISearchHistoryRepository : IMongoRepositoryBase<SearchHistoryEntity>
{
    Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit);
    Task<SearchHistoryEntity> GetByQueryAsync(string query);
    Task<List<string>> GetSearchByUserIdAsync(Guid userId, int limit);
}
