using MongoDB.Driver;

namespace GOCAP.Repository.Intention;

public interface IUnitOfWorkMongo : IAsyncDisposable
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    IClientSessionHandle? GetSession();
}
