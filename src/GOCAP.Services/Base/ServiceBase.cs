namespace GOCAP.Services;

internal abstract class ServiceBase<T>(
    IRepositoryBase<T> _repository, 
    ILogger<ServiceBase<T>> _logger
    ) : IServiceBase<T> where T : class
{
    public virtual async Task<T> AddAsync(T domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(T).Name);
        return await _repository.AddAsync(domain);
    }

    public virtual async Task<OperationResult> AddRangeAsync(IEnumerable<T> domains)
    {
        _logger.LogInformation("Start adding many entities of type");
        return new OperationResult(await _repository.AddRangeAsync(domains));
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        return await _repository.GetByIdsAsync(ids, fieldsName);
    }

    public virtual async Task<QueryResult<T>> GetByPageAsync(QueryInfo queryInfo)
    {
        return await _repository.GetByPageAsync(queryInfo);
    }
    
    public virtual async Task<OperationResult> UpdateAsync(Guid id, T domain)
    {
        _logger.LogInformation("Start updating entity of type {EntityType}.", typeof(T).Name);
        return new OperationResult(await _repository.UpdateAsync(id, domain));
    }
    
    public virtual async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(T).Name);
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
