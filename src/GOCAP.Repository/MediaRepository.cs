namespace GOCAP.Repository;

[RegisterService(typeof(IMediaRepository))]
internal class MediaRepository(AppMongoDbContext _context, IMapper _mapper) : MongoRepositoryBase<Media, MediaEntity>(_context, _mapper), IMediaRepository
{
}
