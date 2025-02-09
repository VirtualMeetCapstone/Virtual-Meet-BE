namespace GOCAP.Database;

public class AppMongoDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<MediaEntity> Medias { get; set; }
    public IMongoCollection<CommentEntity> Comments { get; set; }
    public AppMongoDbContext(string databaseName, string connectionString)
    {
        var _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);

        Medias = GetCollection<MediaEntity>();
        Comments = GetCollection<CommentEntity>();
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
