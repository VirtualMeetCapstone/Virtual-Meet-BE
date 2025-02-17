namespace GOCAP.Services;

internal abstract class ServiceBase<TDomain, TEntity>(
    IRepositoryBase<TEntity> _repository,
    IMapper _mapper,
    ILogger<ServiceBase<TDomain, TEntity>> _logger
    ) : IServiceBase<TDomain> where TDomain : class
{
    public virtual async Task<TDomain> AddAsync(TDomain domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(TDomain).Name);
        if (domain is DateTrackingBase objectBase)
        {
            objectBase.InitCreation();
        }
        var entity = _mapper.Map<TEntity>(domain);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<TDomain>(result);
    }

    public virtual async Task<OperationResult> AddRangeAsync(IEnumerable<TDomain> domains)
    {
        _logger.LogInformation("Start adding many entities of type");
        var entities = _mapper.Map<IEnumerable<TEntity>>(domains);
        return new OperationResult(await _repository.AddRangeAsync(entities));
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public virtual async Task<int> GetCountAsync(Expression<Func<TDomain, bool>>? condition = null)
    {
        condition ??= x => true;
        var entities = _mapper.Map<Expression<Func<TEntity, bool>>>(condition);
        return await _repository.GetCountAsync(entities);
    }

    public virtual async Task<TDomain> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<TDomain>(entity);
    }

    public virtual async Task<IEnumerable<TDomain>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var entities = await _repository.GetByIdsAsync(ids, fieldsName);
        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public virtual async Task<QueryResult<TDomain>> GetByPageAsync(QueryInfo queryInfo)
    {
        var entities = await _repository.GetByPageAsync(queryInfo);
        return _mapper.Map<QueryResult<TDomain>>(entities);
    }

    public virtual async Task<OperationResult> UpdateAsync(Guid id, TDomain domain)
    {
        _logger.LogInformation("Start updating entity of type {EntityType}.", typeof(TDomain).Name);
        if (domain is DateTrackingBase objectBase)
        {
            objectBase.UpdateModify();
        }
        var entity = _mapper.Map<TEntity>(domain);
        return new OperationResult(await _repository.UpdateAsync(entity));
    }

    public virtual async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(TDomain).Name);
        var result = await _repository.DeleteByIdAsync(id);
        return new OperationResult(result > 0);
    }

    public virtual async Task<OperationResult> CheckExistAsync(Guid id, string name)
    {
        return new OperationResult(await _repository.CheckExistAsync(id, name));
    }

    public virtual async Task<OperationResult> CheckExistAsync(Guid id)
    {
        return new OperationResult(await _repository.CheckExistAsync(id));
    }
}
