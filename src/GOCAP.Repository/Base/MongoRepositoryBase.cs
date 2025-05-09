﻿
namespace GOCAP.Repository;

internal abstract class MongoRepositoryBase<TEntity>
    (AppMongoDbContext _context) : IMongoRepositoryBase<TEntity>
    where TEntity : EntityMongoBase
{
    private readonly IMongoCollection<TEntity> _collection = _context.GetCollection<TEntity>();

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
    {
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
    
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var entities = await _collection.Find(FilterDefinition<TEntity>.Empty).ToListAsync();

        return entities;
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        var filter = Builders<TEntity>.Filter.Eq("Id", id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
        return entity;
    }

    public Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var filter = Builders<TEntity>.Filter.In("Id", ids);
        var projection = Builders<TEntity>.Projection.Include(fieldsName);
        var results = await _collection.Find(filter).Project<TEntity>(projection).ToListAsync();
        return results;
    }

    public virtual async Task<QueryResult<TEntity>> GetByPageAsync(QueryInfo queryInfo)
    {
        var filter = Builders<TEntity>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            filter = Builders<TEntity>.Filter.Regex("Name", new BsonRegularExpression(queryInfo.SearchText, "i")); // Find no distinct lowercase or uppercase
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

        return new QueryResult<TEntity>
        {
            Data = entities,
            TotalCount = totalItems
        };
    }

    public Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition)
    {
        throw new InvalidOperationException();  
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity)
    {
        var filter = Builders<TEntity>.Filter.Eq("Id", entity.Id);
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
