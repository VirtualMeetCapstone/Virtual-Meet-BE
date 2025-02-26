
namespace GOCAP.Services;

[RegisterService(typeof(IUserRoleService))]
internal class UserRoleService(
    IUserRoleRepository _repository,
    IMapper _mapper,
    ILogger<UserRoleService> _logger) : ServiceBase<UserRole, UserRoleEntity>(_repository, _mapper, _logger), IUserRoleService
{
    private readonly IMapper _mapper = _mapper;
    public async Task<UserRole> AssignRoleToUser(UserRole domain)
    {
        var entity = _mapper.Map<UserRoleEntity>(domain);
        var result = await _repository.AssignRoleToUser(entity);
        return _mapper.Map<UserRole>(result);
    }
}
