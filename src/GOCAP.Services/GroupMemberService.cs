namespace GOCAP.Services;

[RegisterService(typeof(IGroupMemberService))]
internal class GroupMemberService(
    IGroupMemberRepository _repository,
    ILogger<GroupMemberService> _logger
    ) : ServiceBase<GroupMember>(_repository, _logger), IGroupMemberService
{
}
