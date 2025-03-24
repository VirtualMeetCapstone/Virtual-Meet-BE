
namespace GOCAP.Repository;

[RegisterService(typeof(ISearchHistoryRepository))]
internal class SearchHistoryRepository(AppMongoDbContext context) : MongoRepositoryBase<SearchHistoryEntity>(context), ISearchHistoryRepository
{
    private readonly AppMongoDbContext _context = context;

    public async Task<SearchHistoryEntity> GetByQueryAsync(string query)
    {
        var filter = Builders<SearchHistoryEntity>.Filter.Eq(x => x.Query, query);
        return await _context.SearchHistories.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<string>> GetPopularSearchSuggestionsAsync(string prefix, int limit)
    {
        var filter = Builders<SearchHistoryEntity>
            .Filter.Regex("Query", new BsonRegularExpression($"^{prefix}", "i"));

        var results = await _context.SearchHistories
                                .Aggregate()
                                .Match(filter)
                                .Group(x => x.Query, g => new { Query = g.Key, Count = g.Count() })
                                .SortByDescending(x => x.Count)
                                .Limit(limit)
                                .Project(x => x.Query)
                                .ToListAsync();

        return results;
    }
}
