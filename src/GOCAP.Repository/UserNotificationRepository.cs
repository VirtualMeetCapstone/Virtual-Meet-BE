
namespace GOCAP.Repository;

[RegisterService(typeof(IUserNotificationRepository))]
internal class UserNotificationRepository
    (AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<UserNotification, UserNotificationEntity>(context, mapper), IUserNotificationRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<List<UserNotification>> GetNotificationsByUserIdAsync(Guid userId)
    {
        var result = await ( from u in _context.Users
                             join n in _context.UserNotifications
                             on u.Id equals n.ReferenceId
                             where u.Id == userId
                             select new UserNotification
                             {
                                 Id = n.Id,
                                 Sender = new User
                                 {
                                     Name = u.Name,
                                     Picture = u.Picture
                                 },
                                 Content = n.Content,
                                 Type = n.Type,
                                 IsRead = n.IsRead
                             }
                          )
                          .ToListAsync();
        return result;  
    }
}