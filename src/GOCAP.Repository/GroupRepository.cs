
namespace GOCAP.Repository;

[RegisterService(typeof(IGroupRepository))]
internal class GroupRepository(AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<Group, GroupEntity>(context, mapper), IGroupRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<QueryResult<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId)
    {
        var groupEntities = await _context.Groups.Where(g => g.OwnerId == userId)
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

    public override async Task<bool> UpdateAsync(Guid id, Group domain)
    {
        var entity = await GetEntityByIdAsync(id);
        domain.UpdateModify();
        entity.Name = domain.Name;
        entity.Picture = domain.Picture;
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}
