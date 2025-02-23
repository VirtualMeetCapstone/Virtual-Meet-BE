namespace GOCAP.Services;

[RegisterService(typeof(IRoleService))]
internal class RoleService (
    IRoleRepository _repository,
    IMapper _mapper,
    ILogger<RoleService> _logger) : ServiceBase<Role, RoleEntity>(_repository, _mapper, _logger), IRoleService
{
}
