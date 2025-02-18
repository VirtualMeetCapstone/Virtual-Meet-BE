
namespace GOCAP.Repository;

[RegisterService(typeof(IGroupMemberRepository))]
internal class GroupMemberRepository(AppSqlDbContext context) : SqlRepositoryBase<GroupMemberEntity>(context), IGroupMemberRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<int> DeleteByGroupIdAsync(Guid groupId)
    {
        return await _context.GroupMembers
                                .Where(rm => rm.GroupId == groupId)
                                .ExecuteDeleteAsync();
    }

    public async Task<GroupMemberEntity?> GetByGroupMemberAsync(Guid groupId, Guid userId)
    {
        var entity = await _context.GroupMembers.FirstOrDefaultAsync
                                                (gm => gm.GroupId == groupId
                                                 && gm.UserId == userId);
        return entity;
    }
}
