namespace GOCAP.Tests.Unit;

public abstract class RepositoryTestsBase
{
    protected readonly Mock<IAppConfiguration> _configurationMock;
    protected readonly AppSqlDbContext _dbContext;
    protected RepositoryTestsBase()
    {
        // Mock configuration
        _configurationMock = new Mock<IAppConfiguration>();
        _configurationMock.Setup(c => c.GetEnvironment()).Returns(AppConstants.Enviroments.Test); 
        _configurationMock.Setup(c => c.GetSqlServerConnectionString()).Returns(AppConstants.Enviroments.Test);

        // Use InMemoryDatabase for test environment
        var options = new DbContextOptionsBuilder<AppSqlDbContext>()
            .UseInMemoryDatabase($"{AppConstants.Enviroments.Test}_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppSqlDbContext(options, _configurationMock.Object);
    }
}
