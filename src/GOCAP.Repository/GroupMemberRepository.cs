
namespace GOCAP.Repository;

[RegisterService(typeof(IGroupMemberRepository))]
internal class GroupMemberRepository(AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<GroupMember, GroupMemberEntity>(context, mapper), IGroupMemberRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<int> DeleteByGroupIdAsync(Guid id)
    {
        return await _context.GroupMembers
                                .Where(rm => rm.GroupId == id)
                                .ExecuteDeleteAsync();
    }

    public async Task<GroupMember?> GetByGroupMemberAsync(Guid groupId, Guid userId)
    {
        var entity = await _context.GroupMembers.FirstOrDefaultAsync
                                                (gm => gm.GroupId == groupId
                                                 && gm.UserId == userId);
        return _mapper.Map<GroupMember?>(entity);
    }
}
