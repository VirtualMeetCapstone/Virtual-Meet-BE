namespace GOCAP.Repository.Intention;

public interface ILogoRepository : IMongoRepositoryBase<LogoEntity>
{
    Task<LogoEntity> GetAsync();
}
