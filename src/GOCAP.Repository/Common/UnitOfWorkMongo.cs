namespace GOCAP.Repository;

public class UnitOfWorkMongo(AppMongoDbContext context) : IUnitOfWorkMongo
{
    private readonly AppMongoDbContext _context = context;
    private IClientSessionHandle? _session;

    public IMongoDatabase Database => _context.GetDatabase();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        var client = _context.GetClient();
        _session = await client.StartSessionAsync(cancellationToken: cancellationToken);
        _session.StartTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            throw new InvalidOperationException("No active transaction.");
        }

        await _session.CommitTransactionAsync(cancellationToken);
        _session.Dispose();
        _session = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            throw new InvalidOperationException("No active transaction.");
        }

        await _session.AbortTransactionAsync(cancellationToken);
        _session.Dispose();
        _session = null;
    }

    public IClientSessionHandle? GetSession() => _session;

    public ValueTask DisposeAsync()
    {
        _session?.Dispose();
        _session = null;
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}