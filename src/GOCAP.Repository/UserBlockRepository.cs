namespace GOCAP.Repository;

[RegisterService(typeof(IUserBlockRepository))]
internal class UserBlockRepository
	(AppSqlDbContext context) : SqlRepositoryBase<UserBlockEntity>(context), IUserBlockRepository
{
}
