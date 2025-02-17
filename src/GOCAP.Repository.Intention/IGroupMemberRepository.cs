namespace GOCAP.Repository.Intention;

public interface IGroupMemberRepository : ISqlRepositoryBase<GroupMemberEntity>
{
    Task<int> DeleteByGroupIdAsync(Guid groupId);
    Task<GroupMemberEntity?> GetByGroupMemberAsync(Guid groupId, Guid userId);
}
