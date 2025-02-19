using Microsoft.EntityFrameworkCore.Storage;

namespace GOCAP.Repository;

public class UnitOfWork(AppSqlDbContext context) : IUnitOfWork
{
    private readonly AppSqlDbContext _context = context;
    private IDbContextTransaction? _transaction;
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction.");
        }

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction.");
        }

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void TrackEntity<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public ValueTask DisposeAsync()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

}
