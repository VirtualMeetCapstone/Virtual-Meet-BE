namespace GOCAP.Repository;

[RegisterService(typeof(ILogoRepository))]
internal class LogoRepository(AppMongoDbContext _context)
    : MongoRepositoryBase<LogoEntity>(_context), ILogoRepository
{
    private readonly AppMongoDbContext _context = _context;

    public async Task<LogoEntity> GetAsync()
        => await _context.Logos.Find(_ => true).FirstOrDefaultAsync();
}



