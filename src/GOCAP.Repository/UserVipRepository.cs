namespace GOCAP.Repository
{
    [RegisterService(typeof(IUserVipRepository))]
    internal class UserVipRepository(AppMongoDbContext _context)
        : MongoRepositoryBase<UserVip>(_context), IUserVipRepository
    {
        public async Task AddOrUpdateUserVipAsync(Guid userId, string level, DateTime? expireAt = null)
        {
            var filter = Builders<UserVip>.Filter.Eq(x => x.UserId, userId);

            var existing = await _context.UserVips.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Level = level;
                existing.ExpireAt = expireAt;
                existing.LastModifyTime = DateTime.Now.Ticks;
                await _context.UserVips.ReplaceOneAsync(filter, existing);
            }
            else
            {
                var newUserVip = new UserVip
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Level = level,
                    ExpireAt = expireAt,
                    CreateTime = DateTime.Now.Ticks
                };

                await _context.UserVips.InsertOneAsync(newUserVip);
            }
        }


        public async Task<UserVip> GetUserVipLevel(Guid userId)
        {
            var userVip = await _context.UserVips
                .Find(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            var result = new UserVip
            {
                Level = userVip?.Level ?? "free",
                ExpireAt = userVip?.ExpireAt
            };

            return result;

        }
    }
}