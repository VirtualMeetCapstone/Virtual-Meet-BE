namespace GOCAP.Services;

[RegisterService(typeof(IPermissionService))]
internal class PermissionService(IPermissionRepository _repository, IMapper _mapper, ILogger<PermissionService> _logger) : ServiceBase<Permission, PermissionEntity>(_repository, _mapper, _logger), IPermissionService
{
    private readonly IMapper _mapper = _mapper;
    public async Task<List<Permission>> GetUserPermissionsByUserIdAsync(Guid userId)
    {
        var result = await _repository.GetUserPermissionsByUserIdAsync(userId);
        return _mapper.Map<List<Permission>>(result);
    }
}
