namespace GOCAP.Repository;

internal abstract class SqlRepositoryBase<TEntity>
    (AppSqlDbContext _context) : ISqlRepositoryBase<TEntity>
    where TEntity : EntitySqlBase
{
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var entities = await _context.Set<TEntity>().ToListAsync();
        return entities;
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition)
    {
        if (condition == null)
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        var entities = await _context.Set<TEntity>().ToListAsync();

        return entities.Count(condition.Compile());
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id) ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[]? includes = null, CancellationToken cancellationToken = default)
    {
        if (includes == null)
        {
            return await GetByIdAsync(id);
        }

        var query = _context.Set<TEntity>().AsQueryable();
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        var entity = await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity ?? throw new ResourceNotFoundException($"Entity with {id} was not found.");
    }

    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var entities = await _context.Set<TEntity>().Where(domain => ids.Contains(EF.Property<Guid>(domain, fieldsName))).ToListAsync();
        return entities;
    }

    public virtual async Task<QueryResult<TEntity>> GetByPageAsync(QueryInfo queryInfo)
    {
        var query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            query = GenerateWhereString(query, queryInfo);
        }

        if (!string.IsNullOrWhiteSpace(queryInfo.OrderBy))
        {
            query = queryInfo.OrderType == OrderType.Ascending
                ? query.OrderBy(e => EF.Property<object>(e, queryInfo.OrderBy))
                : query.OrderByDescending(e => EF.Property<object>(e, queryInfo.OrderBy));
        }

        int totalItems = 0;

        // Count number of record if required
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await query.CountAsync();
        }

        var items = await query.Skip(queryInfo.Skip).Take(queryInfo.Top).ToListAsync();

        return new QueryResult<TEntity>
        {
            Data = items,
            TotalCount = totalItems
        };
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        _context.Set<TEntity>().Remove(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> DeleteByEntityAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> CheckExistAsync(Guid id, string name)
    {
        var exists = await _context.Set<TEntity>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") == id
            && EF.Property<string>(e, "Name") == name
        );
        return exists;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id)
    {
        var exists = await _context.Set<TEntity>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") == id
        );
        return exists;
    }

    protected virtual IQueryable<TEntity> GenerateWhereString(IQueryable<TEntity> query, QueryInfo queryInfo) => query;

    public async Task<TEntity> GetByIdAsync(Guid id, bool isAsNoTracking = false)
    {
        if (isAsNoTracking)
        {
            return await GetByIdAsync(id);
        }
        return await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id) ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
    }
}
