
namespace GOCAP.Repository;

[RegisterService(typeof(IGroupRepository))]
internal class GroupRepository(AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<Group, GroupEntity>(context, mapper), IGroupRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<List<Group>> GetByUserIdWithPagingAsync(QueryInfo queryInfo, Guid userId)
    {
        var groupQuery = _context.Groups.AsQueryable();
    }
}
