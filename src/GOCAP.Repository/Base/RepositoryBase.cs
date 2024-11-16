using AutoMapper;

namespace GOCAP.Repository;

internal abstract class RepositoryBase<TDomain, TEntity>
    ( DbContextBase _context, IMapper _mapper) : IRepositoryBase<TDomain> 
    where TDomain : DomainBase
    where TEntity : EntityBase
{
    public virtual async Task<TDomain> AddAsync(TDomain domain)
    {
        await _context.Set<TDomain>().AddAsync(domain);
        await _context.SaveChangesAsync();
        return domain;
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<TDomain> domains)
    {
        await _context.Set<TDomain>().AddRangeAsync(domains);
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync()
    {
        var entities = await _context.Set<TEntity>().ToListAsync();
        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public virtual async Task<TDomain> GetByIdAsync(Guid id)
    {
        var entity = await _context.Set<TEntity>().FindAsync(id) ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
        return _mapper.Map<TDomain>(entity); ;
    }

    public virtual async Task<IEnumerable<TDomain>> GetByIdsAsync(List<Guid> ids, string fieldsName)
    {
        var entities = await _context.Set<TEntity>().Where(domain => ids.Contains(EF.Property<Guid>(domain, fieldsName))).ToListAsync();
        return _mapper.Map<IEnumerable<TDomain>>(entities);
    }

    public virtual async Task<QueryResult<TDomain>> GetByPageAsync(QueryInfo queryInfo)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryInfo.Search))
        {
            query = query.Where(e => EF.Functions.Like(EF.Property<string>(e, "Name"), $"%{queryInfo.Search}%"));
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

        return new QueryResult<TDomain>
        {
            Data = _mapper.Map<IEnumerable<TDomain>>(items),
            TotalCount = totalItems
        };
    }

    public virtual async Task<bool> UpdateAsync(Guid id, TDomain domain)
    {
        var existingEntity = await _context.Set<TDomain>().FindAsync(id);
        if (existingEntity == null)
        {
            return false;
        }
        _context.Entry(existingEntity).CurrentValues.SetValues(domain);
        return await _context.SaveChangesAsync() > 0;
    }

    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity = await _context.Set<TDomain>().FindAsync(id) ?? throw new ResourceNotFoundException($"Entity with id {id} not found");
        _context.Set<TDomain>().Remove(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> CheckExistAsync(Guid id, string name)
    {
        var exists = await _context.Set<TDomain>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") != id
            && EF.Property<string>(e, "Name") == name
        );
        return exists;
    }

    public virtual async Task<bool> CheckExistAsync(Guid id)
    {
        var exists = await _context.Set<TDomain>().AnyAsync(
            e => EF.Property<Guid>(e, "Id") != id
        );
        return exists;
    }
}
