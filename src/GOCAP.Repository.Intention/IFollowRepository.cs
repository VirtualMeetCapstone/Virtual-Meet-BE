namespace GOCAP.Repository.Intention;

public interface IFollowRepository : ISqlRepositoryBase<Follow>
{
    Task<Follow?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId);
}
