namespace GOCAP.Repository.Intention;

public interface ISqlRepositoryBase<TEntity> : IRepositoryBase<TEntity>
{
    Task<TEntity> GetByIdAsync(Guid id, bool isAsNoTracking = false);
    Task<int> DeleteByEntityAsync(TEntity entity);
}
