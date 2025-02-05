namespace GOCAP.Repository;

internal abstract class SqlRepositoryBase<TDomain, TEntity>
    (AppSqlDbContext _context, IMapper _mapper) : ISqlRepositoryBase<TDomain>
    where TDomain : DomainBase
    where TEntity : EntitySqlBase
{
    public virtual async Task<TDomain> AddAsync(TDomain domain)
    {
        var entity = _mapper.Map<TEntity>(domain);
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return domain;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TDomain> domains)
    {
        var entity = _mapper.Map<IEnumerable<TEntity>>(domains);
        await _context.Set<TEntity>().AddRangeAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync()
    {
        var entities = await _context.Set<TEntity>().ToListAsync();
        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public async Task<int> GetCountAsync(Expression<Func<TDomain, bool>>? condition)
    {
        if (condition == null)
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        var entities = await _context.Set<TEntity>().ToListAsync();
        var mappedEntities = _mapper.Map<IEnumerable<TDomain>>(entities);

        return mappedEntities.Count(condition.Compile());
    }

    public virtual async Task<TDomain> GetByIdAsync(Guid id)
    {
        var entity = await GetEntityByIdAsync(id);
        return _mapper.Map<TDomain>(entity);
    }

    public virtual async Task<IEnumerable<TDomain>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var entities = await _context.Set<TEntity>().Where(domain => ids.Contains(EF.Property<Guid>(domain, fieldsName))).ToListAsync();
        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public virtual async Task<QueryResult<TDomain>> GetByPageAsync(QueryInfo queryInfo)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        GenerateWhereString(query, queryInfo);

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

        return new QueryResult<TDomain>
        {
            Data = _mapper.Map<IEnumerable<TDomain>>(items),
            TotalCount = totalItems
        };
    }

    public virtual async Task<bool> UpdateAsync(Guid id, TDomain domain)
    {
        var entity = await GetEntityByIdAsync(id);
        _mapper.Map(domain, entity);
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity = await GetEntityByIdAsync(id);
        _context.Set<TEntity>().Remove(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> CheckExistAsync(Guid id, string name)
    {
        var exists = await _context.Set<TEntity>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") != id
            && EF.Property<string>(e, "Name") == name
        );
        return exists;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id)
    {
        var exists = await _context.Set<TEntity>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") != id
        );
        return exists;
    }

    protected virtual async Task<TEntity> GetEntityByIdAsync(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id) ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
    }

    protected virtual IQueryable<TEntity> GenerateWhereString(IQueryable<TEntity> query, QueryInfo queryInfo) => query;
}
