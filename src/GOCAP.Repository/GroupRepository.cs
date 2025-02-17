namespace GOCAP.Repository;

[RegisterService(typeof(IGroupRepository))]
internal class GroupRepository(AppSqlDbContext context, IMapper _mapper)
    : SqlRepositoryBase<GroupEntity>(context)
    , IGroupRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<QueryResult<GroupEntity>> GetByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var groups = await _context.Groups.Where(g => g.OwnerId == userId)
                                                 .OrderByDescending(g => g.CreateTime)
                                                 .Skip(queryInfo.Skip)
                                                 .Take(queryInfo.Top)
                                                 .ToListAsync();
        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await _context.Groups.CountAsync();
        }

        return new QueryResult<GroupEntity>
        {
            Data = groups,
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

    public override async Task<bool> UpdateAsync(GroupEntity groupEntity)
    {
        var entity = await GetEntityByIdAsync(groupEntity.Id);
        entity.Name = groupEntity.Name;
        entity.Picture = groupEntity.Picture;
        if (groupEntity.OwnerId != Guid.Empty)
        {
            entity.OwnerId = groupEntity.OwnerId;
        }
        entity.LastModifyTime = DateTime.UtcNow.Ticks;
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}