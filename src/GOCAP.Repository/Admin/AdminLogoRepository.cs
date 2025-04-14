namespace GOCAP.Repository;
[RegisterService(typeof(IAdminLogoRepository))]
internal class AdminLogoRepository(AppMongoDbContext _context, IBlobStorageService _blobStorageService)
    : MongoRepositoryBase<AdminLogoEntity>(_context), IAdminLogoRepository
{
    private readonly IMongoCollection<AdminLogoEntity> _collection = _context.AdminLogos;

    public async Task<AdminLogoEntity> UpdateAsync(AdminLogoEntity entity)
    {
        var logo = await _collection.Find(_ => true).FirstOrDefaultAsync();

        if (logo == null)
        {
            entity.CreateTime = DateTime.UtcNow.Ticks;
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        // Xóa file cũ nếu có
        if (!string.IsNullOrEmpty(logo.Picture))
        {
            var oldMedia = JsonHelper.Deserialize<Media>(logo.Picture);
            if (!string.IsNullOrEmpty(oldMedia?.Url))
            {
                await _blobStorageService.DeleteFilesByUrlsAsync([oldMedia.Url]);
            }
        }

        entity.Id = logo.Id;
        entity.LastModifyTime = DateTime.UtcNow.Ticks;

        var update = Builders<AdminLogoEntity>.Update
            .Set(x => x.Picture, entity.Picture)
            .Set(x => x.LastModifyTime, entity.LastModifyTime);

        await _collection.UpdateOneAsync(x => x.Id == logo.Id, update);

        return entity;
    }

    public async Task<AdminLogoEntity> GetAsync()
        => await _collection.Find(_ => true).FirstOrDefaultAsync();
}



