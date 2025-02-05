namespace GOCAP.Repository.Intention;

public interface IGroupMemberRepository : ISqlRepositoryBase<GroupMember>
{
    Task<int> DeleteByGroupIdAsync(Guid groupId);
    Task<GroupMember?> GetByGroupMemberAsync(Guid groupId, Guid userId);
}
