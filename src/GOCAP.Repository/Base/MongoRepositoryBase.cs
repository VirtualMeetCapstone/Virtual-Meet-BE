namespace GOCAP.Repository;

internal abstract class MongoRepositoryBase<T>(
    GoCapMongoDbContext _context, 
    ILogger<MongoRepositoryBase<T>> _logger
    ) : IRepositoryBase<T> where T : class
{
    private readonly IMongoCollection<T> _collection = _context.GetCollection<T>(typeof(T).Name);

    public virtual async Task<T> AddAsync(T domain)
    {
        await _collection.InsertOneAsync(domain);
        return domain;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<T> domains)
    {
        await _collection.InsertManyAsync(domains);
        return true;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id, string name)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq("Id", id),
            Builders<T>.Filter.Eq("Name", name)
        );
        var exists = await _collection.Find(filter).AnyAsync();
        return exists;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var exists = await _collection.Find(filter).AnyAsync();
        return exists;
    }

    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var result = await _collection.DeleteOneAsync(filter);
        if (result.DeletedCount == 0)
        {
            _logger.LogError("Entity was not found");
            throw new ResourceNotFoundException($"Entity with id {id} not found");
        }
        return (int)result.DeletedCount;
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();
        if (entity == null)
        {
            _logger.LogError("Entity was not found");
            throw new ResourceNotFoundException($"Entity with id {id} not found");
        }
        return entity;
    }

    public virtual async Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var filter = Builders<T>.Filter.In("Id", ids);
        var projection = Builders<T>.Projection.Include(fieldsName);
        var results = await _collection.Find(filter).Project<T>(projection).ToListAsync();

        return results;
    }

    public virtual async Task<QueryResult<T>> GetByPageAsync(QueryInfo queryInfo)
    {
        var filter = Builders<T>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(queryInfo.Search))
        {
            filter = Builders<T>.Filter.Regex("Name", new BsonRegularExpression(queryInfo.Search, "i")); // Find no distinct lowercase or uppercase
        }

        var query = _collection.Find(filter);

        if (!string.IsNullOrWhiteSpace(queryInfo.OrderBy))
        {
            var sort = queryInfo.OrderType == OrderType.Ascending
                ? Builders<T>.Sort.Ascending(queryInfo.OrderBy)
                : Builders<T>.Sort.Descending(queryInfo.OrderBy);

            query = query.Sort(sort);
        }

        int totalItems = 0;

        // Count number of records if required
        if (queryInfo.NeedTotalCount)
        {
            totalItems = (int)await _collection.CountDocumentsAsync(filter);
        }

        var items = await query.Skip(queryInfo.Skip).Limit(queryInfo.Top).ToListAsync();

        return new QueryResult<T>
        {
            Data = items,
            TotalCount = totalItems
        };
    }

    public virtual async Task<bool> UpdateAsync(Guid id, T domain)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var result = await _collection.ReplaceOneAsync(filter, domain);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
