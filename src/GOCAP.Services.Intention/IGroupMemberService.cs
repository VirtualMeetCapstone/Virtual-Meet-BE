namespace GOCAP.Services.Intention;

public interface IGroupMemberService : IServiceBase<GroupMember>
{
    Task<OperationResult> AddOrRemoveMemberAsync(GroupMember domain);
}
