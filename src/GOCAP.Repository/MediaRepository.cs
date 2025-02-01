namespace GOCAP.Repository;

[RegisterService(typeof(IMediaRepository))]
internal class MediaRepository(AppMongoDbContext context, IMapper mapper) : MongoRepositoryBase<Media, MediaEntity>(context, mapper), IMediaRepository
{
}
