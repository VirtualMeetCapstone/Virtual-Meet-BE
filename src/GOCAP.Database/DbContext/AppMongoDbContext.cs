namespace GOCAP.Database;

public class AppMongoDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<CommentEntity> Comments { get; set; }
    public IMongoCollection<CommentReactionEntity> CommentReactions { get; set; }
    public IMongoCollection<SearchHistoryEntity> SearchHistories { get; set; }
    public AppMongoDbContext(string databaseName, string connectionString)
    {
        var _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
        Comments = GetCollection<CommentEntity>();
        CommentReactions = GetCollection<CommentReactionEntity>();
        SearchHistories = GetCollection<SearchHistoryEntity>();
    }


    /// <summary>
    /// Get collection base on BsonCollection attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IMongoCollection<T> GetCollection<T>() where T : class
    {
        var collectionName = typeof(T)
            .GetCustomAttributes(typeof(BsonCollectionAttribute), true)
            .FirstOrDefault() is BsonCollectionAttribute collectionAttribute
            ? collectionAttribute.CollectionName
            : typeof(T).Name;

        return _database.GetCollection<T>(collectionName);
    }

}
