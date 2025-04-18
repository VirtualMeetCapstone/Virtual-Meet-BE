namespace GOCAP.Repository;

[RegisterService(typeof(INotificationRepository))]
internal class NotificationRepository
    (AppMongoDbContext context, IMapper mapper) : MongoRepositoryBase<NotificationEntity>(context), INotificationRepository
{
    private readonly AppMongoDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<QueryResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        var filter = Builders<NotificationEntity>.Filter.AnyEq(n => n.UserIds, userId);

        var notifications = await _context.Notifications
                                          .Find(filter)
                                          .SortByDescending(n => n.CreateTime)
                                          .Skip(queryInfo.Skip)
                                          .Limit(queryInfo.Top)
                                          .ToListAsync();

        var unreadFilter = Builders<NotificationEntity>.Filter.And(
            Builders<NotificationEntity>.Filter.AnyEq(n => n.UserIds, userId),
            Builders<NotificationEntity>.Filter.Eq(n => n.IsRead, false)
        );

        var unreadCount = await _context.Notifications.CountDocumentsAsync(unreadFilter);

        return new QueryResult<Notification>
        {
            Data = _mapper.Map<List<Notification>>(notifications),
            TotalCount = (int)unreadCount
        };
    }



    public async Task<bool> MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var filter = Builders<NotificationEntity>.Filter.And(
            Builders<NotificationEntity>.Filter.AnyEq(n => n.UserIds, userId),
            Builders<NotificationEntity>.Filter.Eq(n => n.Id, notificationId)
        );

        var update = Builders<NotificationEntity>.Update.Set(n => n.IsRead, true);

        var result = await _context.Notifications.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

}