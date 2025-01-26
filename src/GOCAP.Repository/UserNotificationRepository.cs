namespace GOCAP.Repository;

[RegisterService(typeof(IUserNotificationRepository))]
internal class UserNotificationRepository
    (AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<UserNotification, UserNotificationEntity>(context, mapper), IUserNotificationRepository
{
}