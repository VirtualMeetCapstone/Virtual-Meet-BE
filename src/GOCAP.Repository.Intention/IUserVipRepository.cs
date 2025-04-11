namespace GOCAP.Repository.Intention
{
    public interface IUserVipRepository : IMongoRepositoryBase<UserVip>
    {
        Task AddOrUpdateUserVipAsync(Guid userId, string level, DateTime? expireAt = null);
        Task<UserVip> GetUserVipLevel(Guid userId);
    }
}
