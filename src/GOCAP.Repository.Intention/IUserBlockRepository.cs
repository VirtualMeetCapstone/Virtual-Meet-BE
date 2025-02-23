namespace GOCAP.Repository;

public interface IUserBlockRepository : ISqlRepositoryBase<UserBlockEntity>
{
	Task<UserBlock?> GetBlockOrBlockedAsync(UserBlock model);
}
