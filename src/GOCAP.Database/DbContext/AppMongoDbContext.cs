namespace GOCAP.Database;

public class AppMongoDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<MediaEntity> Medias { get; set; }
    public AppMongoDbContext(string databaseName, string connectionString)
    {
        var _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);

        Medias = GetCollection<MediaEntity>(typeof(MediaEntity).Name);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

}
