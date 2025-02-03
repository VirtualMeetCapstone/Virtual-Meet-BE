
namespace GOCAP.Repository;

[RegisterService(typeof(IGroupMemberRepository))]
internal class GroupMemberRepository(AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<GroupMember, GroupMemberEntity>(context, mapper), IGroupMemberRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<int> DeleteByGroupIdAsync(Guid id)
    {
        return await _context.GroupMembers
                                .Where(rm => rm.GroupId == id)
                                .ExecuteDeleteAsync();
    }
}
