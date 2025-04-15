namespace GOCAP.Repository
{
    [RegisterService(typeof(IUserVipRepository))]
    internal class UserVipRepository(AppMongoDbContext _context)
        : MongoRepositoryBase<UserVip>(_context), IUserVipRepository
    {
        public async Task AddOrUpdateUserVipAsync(Guid userId, int packageId, DateTime? expireAt = null)
        {
            var filter = Builders<UserVip>.Filter.Eq(x => x.UserId, userId);

            var existing = await _context.UserVips.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.PackageId = packageId;
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
                    PackageId = packageId,  
                    ExpireAt = expireAt,
                    CreateTime = DateTime.Now.Ticks,
                };

                await _context.UserVips.InsertOneAsync(newUserVip);
            }
        }


        public async Task<UserVip> GetUserVipLevel(Guid userId)
        {
            var userVip = await _context.UserVips
                .Find(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (userVip == null || (userVip.ExpireAt.HasValue && userVip.ExpireAt.Value <= DateTime.UtcNow))
            {
                return new UserVip
                {
                    PackageId = 0,
                    ExpireAt = null
                };
            }
            return new UserVip
            {
                PackageId = userVip.PackageId,
                ExpireAt = userVip.ExpireAt
            };
        }


    }
}