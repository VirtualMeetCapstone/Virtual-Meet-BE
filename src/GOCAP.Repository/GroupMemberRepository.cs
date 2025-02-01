namespace GOCAP.Repository;

[RegisterService(typeof(IGroupMemberRepository))]
internal class GroupMemberRepository(AppSqlDbContext context, IMapper mapper) : SqlRepositoryBase<GroupMember, GroupMemberEntity>(context, mapper), IGroupMemberRepository
{
}
