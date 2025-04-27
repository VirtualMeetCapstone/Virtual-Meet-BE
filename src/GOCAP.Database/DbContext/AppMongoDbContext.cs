
using GOCAP.Database.MongoDb_Entities;

namespace GOCAP.Database;

public class AppMongoDbContext
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    public IMongoCollection<CommentEntity> Comments { get; set; }
    public IMongoCollection<CommentReactionEntity> CommentReactions { get; set; }
    public IMongoCollection<SearchHistoryEntity> SearchHistories { get; set; }
    public IMongoCollection<MessageEntity> Messages { get; set; }
    public IMongoCollection<NotificationEntity> Notifications { get; set; }
    public IMongoCollection<UserVip> UserVips { get; set; }
    public IMongoCollection<PaymentHistory> PaymentHistorys { get; set; }
    public IMongoCollection<LogoEntity> Logos { get; set; }
    public IMongoCollection<MessagesOutsideRoomEntity> MessagesOutsideRoom { get; set; }
    public IMongoCollection<RoomStatisticsEntity> RoomStatistic { get; set; }
    public IMongoCollection<QuizSessionEntity> QuizSessions { get; set; }
    public IMongoCollection<QuizEntity> Quizzes { get; set; }

    public IMongoCollection<ReportEntity> Reports => _database.GetCollection<ReportEntity>("Reports");


    public AppMongoDbContext(string databaseName, string connectionString)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
        Comments = GetCollection<CommentEntity>();
        CommentReactions = GetCollection<CommentReactionEntity>();
        SearchHistories = GetCollection<SearchHistoryEntity>();
        Messages = GetCollection<MessageEntity>();
        Notifications = GetCollection<NotificationEntity>();
        UserVips = GetCollection<UserVip>();
        PaymentHistorys = GetCollection<PaymentHistory>();
        Logos = GetCollection<LogoEntity>();
        MessagesOutsideRoom = GetCollection<MessagesOutsideRoomEntity>();
        RoomStatistic = GetCollection<RoomStatisticsEntity>();
        QuizSessions= GetCollection<QuizSessionEntity>();
        Quizzes= GetCollection<QuizEntity>();

    }

    public IMongoDatabase GetDatabase() => _database;

    public IMongoClient GetClient() => _client;

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
