namespace GOCAP.Repository;

internal abstract class MongoRepositoryBase<TDomain, TEntity>
    (AppMongoDbContext _context, IMapper _mapper) : IMongoRepositoryBase<TDomain>
    where TDomain : DomainBase
    where TEntity : EntityMongoBase
{
    private readonly IMongoCollection<TEntity> _collection = _context.GetCollection<TEntity>(typeof(TEntity).Name);

    public virtual async Task<TDomain> AddAsync(TDomain domain)
    {
        var entity = _mapper.Map<TEntity>(domain);
        await _collection.InsertOneAsync(entity);
        return domain;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TDomain> domains)
    {
        var entities = _mapper.Map<IEnumerable<TEntity>>(domains);
        await _collection.InsertManyAsync(entities);
        return true;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id, string name)
    {
        var filter = Builders<TEntity>.Filter.And(
            Builders<TEntity>.Filter.Eq("Id", id),
            Builders<TEntity>.Filter.Eq("Name", name)
        );
        var exists = await _collection.Find(filter).AnyAsync();
        return exists;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id)
    {
        var filter = Builders<TEntity>.Filter.Eq("Id", id);
        var exists = await _collection.Find(filter).AnyAsync();
        return exists;
    }

    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var filter = Builders<TEntity>.Filter.Eq("Id", id);
        var result = await _collection.DeleteOneAsync(filter);
        if (result.DeletedCount == 0)
        {
            throw new ResourceNotFoundException($"Entity with id {id} not found");
        }
        return (int)result.DeletedCount;
    }
    
    public virtual async Task<IEnumerable<TDomain>> GetAllAsync()
    {
        var entities = await _collection.Find(FilterDefinition<TEntity>.Empty).ToListAsync();

        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public virtual async Task<TDomain> GetByIdAsync(Guid id)
    {
        var filter = Builders<TEntity>.Filter.Eq("Id", id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
        return _mapper.Map<TDomain>(entity);
    }

    public virtual async Task<IEnumerable<TDomain>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var filter = Builders<TEntity>.Filter.In("Id", ids);
        var projection = Builders<TEntity>.Projection.Include(fieldsName);
        var results = await _collection.Find(filter).Project<TEntity>(projection).ToListAsync();

        return _mapper.Map<IEnumerable<TDomain>>(results);
    }

    public virtual async Task<QueryResult<TDomain>> GetByPageAsync(QueryInfo queryInfo)
    {
        var filter = Builders<TEntity>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(queryInfo.Search))
        {
            filter = Builders<TEntity>.Filter.Regex("Name", new BsonRegularExpression(queryInfo.Search, "i")); // Find no distinct lowercase or uppercase
        }

        var query = _collection.Find(filter);

        if (!string.IsNullOrWhiteSpace(queryInfo.OrderBy))
        {
            var sort = queryInfo.OrderType == OrderType.Ascending
                ? Builders<TEntity>.Sort.Ascending(queryInfo.OrderBy)
                : Builders<TEntity>.Sort.Descending(queryInfo.OrderBy);

            query = query.Sort(sort);
        }

        int totalItems = 0;

        // Count number of records if required
        if (queryInfo.NeedTotalCount)
        {
            totalItems = (int)await _collection.CountDocumentsAsync(filter);
        }

        var entities = await query.Skip(queryInfo.Skip).Limit(queryInfo.Top).ToListAsync();

        return new QueryResult<TDomain>
        {
            Data = _mapper.Map<IEnumerable<TDomain>>(entities),
            TotalCount = totalItems
        };
    }

    public virtual async Task<bool> UpdateAsync(Guid id, TDomain domain)
    {
        var filter = Builders<TEntity>.Filter.Eq("Id", id);
        var entity = _mapper.Map<TEntity>(domain);
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
