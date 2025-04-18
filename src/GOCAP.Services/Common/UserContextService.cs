using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GOCAP.Services;

[RegisterService(typeof(IUserContextService))]
internal class UserContextService (IHttpContextAccessor _httpContextAccessor) : IUserContextService
{
    public Guid Id
    {
        get
        {
            var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        }
    }

    public string Name => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public string Picture => _httpContextAccessor.HttpContext?.User?.FindFirst("picture")?.Value ?? string.Empty;

    public string Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    public bool ValidateUser(Guid userId) => Id == userId;
}
