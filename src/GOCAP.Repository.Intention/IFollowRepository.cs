namespace GOCAP.Repository.Intention;

public interface IFollowRepository : ISqlRepositoryBase<Follow>
{
    Task<bool> CheckExistAsync(Guid followerId, Guid followingId);
    Task<int> DeleteAsync(Guid followerId, Guid followingId);
}
