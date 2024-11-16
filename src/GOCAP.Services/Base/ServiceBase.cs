namespace GOCAP.Services;

internal abstract class ServiceBase<T>(
    IRepositoryBase<T> _repository, 
    ILogger<ServiceBase<T>> _logger
    ) : IServiceBase<T> where T : class
{
    public virtual async Task<T> AddAsync(T domain)
    {
        try
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(T).Name);
            return await _repository.AddAsync(domain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the entity");
            throw new InternalException("An error occurred while adding the entity");
        }
    }

    public virtual async Task<OperationResult> AddRangeAsync(IEnumerable<T> domains)
    {
        try
        {
            _logger.LogInformation("Start adding many entities of type");
            return new OperationResult(await _repository.AddRangeAsync(domains));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding multiple entities");
            throw new InternalException("An error occurred while adding multiple entities");
        }
        
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all entities");
            throw new InternalException("An error occurred while retrieving all entities");
        }
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving entity with id: {id}", id);
            throw new InternalException($"An error occurred while retrieving entity with id: {id}");
        }
    }

    public virtual async Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        try
        {
            return await _repository.GetByIdsAsync(ids, fieldsName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving entity with ids: {ids}", ids);
            throw new ParameterInvalidException("An error occurred while retrieving entities by ids");
        }
    }

    public virtual async Task<QueryResult<T>> GetByPageAsync(QueryInfo queryInfo)
    {
        try
        {
            return await _repository.GetByPageAsync(queryInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while paging entities");
            throw new InternalException("An error occurred while paging entities");
        }
    }
    
    public virtual async Task<OperationResult> UpdateAsync(Guid id, T domain)
    {
        try
        {
            _logger.LogInformation("Startupdating entity of type {EntityType}.", typeof(T).Name);
            return new OperationResult(await _repository.UpdateAsync(id, domain));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Concurrency error while updating entity");
            throw new InternalException("An error occurred while updating the entity");
        }
    }
    
    public virtual async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(T).Name);
            return new OperationResult(await _repository.DeleteByIdAsync(id) > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the entity");
            throw new InternalException("An error occurred while deleting the entity");
        }
    }

    public virtual async Task<OperationResult> CheckExistAsync(Guid id, string name)
    {
        try
        {
            return new OperationResult(await _repository.CheckExistAsync(id, name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking the existence of the entity");
            throw new ParameterInvalidException($"An error occurred while checking the existence of the entity with id: {id} and name: {name}");
        }    
    }

    public virtual async Task<OperationResult> CheckExistAsync(Guid id)
    {
        try
        {
            return new OperationResult(await _repository.CheckExistAsync(id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking the existence of the entity");
            throw new ParameterInvalidException($"An error occurred while checking the existence of the entity with id: {id}");
        }
    }
}
