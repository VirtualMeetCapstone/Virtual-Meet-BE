namespace GOCAP.Repository.Intention;

public interface ISqlRepositoryBase<TEntity> : IRepositoryBase<TEntity>
{
    Task<int> DeleteByEntityAsync(TEntity entity);
}
