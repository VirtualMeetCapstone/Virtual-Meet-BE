namespace GOCAP.Database;

public class GoCapMongoDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<CommentEntity>? Comments { get; set; }
    public GoCapMongoDbContext(string databaseName, string connectionString)
    {
        var _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);

        Comments = GetCollection<CommentEntity>(typeof(CommentEntity).Name);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

}
