namespace GOCAP.Repository;

[RegisterService(typeof(IGroupRepository))]
internal class GroupRepository(AppSqlDbContext context
    , IMapper mapper)
    : SqlRepositoryBase<Group, GroupEntity>(context, mapper)
    , IGroupRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<QueryResult<Group>> GetByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var groupEntities = await _context.Groups.Where(g => g.OwnerId == userId)
                                                 .OrderByDescending(g => g.CreateTime)
                                                 .Skip(queryInfo.Skip)
                                                 .Take(queryInfo.Top)
                                                 .ToListAsync();
        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await _context.Groups.CountAsync();
        }

        return new QueryResult<Group>
        {
            Data = _mapper.Map<List<Group>>(groupEntities),
            TotalCount = totalItems
        };
    }

    public async Task<GroupCount> GetGroupCountsAsync()
    {
        var counts = _context.Groups
               .GroupBy(g => 1)
               .Select(s => new GroupCount
               {
                   Total = s.Count()
               })
               .FirstOrDefaultAsync();
        return await counts ?? new GroupCount();
    }

    public async Task<GroupDetail> GetDetailByIdAsync(Guid id)
    {
        var groupQuery = _context.Groups.AsQueryable();
        var groupEntity = await groupQuery.Include(g => g.Owner)
                                          .Include(g => g.Members)
                                          .ThenInclude(gm => gm.User)
                                          .FirstOrDefaultAsync(g => g.Id == id)
                                          ?? throw new ResourceNotFoundException($"Group with {id} was not                                                          found.");
        var group = new GroupDetail
        {
            Id = groupEntity.Id,
            Name = groupEntity.Name,
            OwnerId = groupEntity.OwnerId,
            Owner = _mapper.Map<User>(groupEntity.Owner),
            Picture = groupEntity.Picture,
            Members = _mapper.Map<List<User>>(groupEntity.Members.Select(gm => gm.User)),
            CreateTime = groupEntity.CreateTime,
            LastModifyTime = groupEntity.LastModifyTime,
        };
        return group;
    }

    public override async Task<bool> UpdateAsync(Guid id, Group domain)
    {
        var entity = await GetEntityByIdAsync(id);
        entity.Name = domain.Name;
        entity.Picture = domain.Picture;
        if (domain.OwnerId != Guid.Empty)
        {
            entity.OwnerId = domain.OwnerId;
        }
        entity.LastModifyTime = DateTime.UtcNow.Ticks;
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}