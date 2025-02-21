using GOCAP.Services.Intention;
using Microsoft.AspNetCore.Http;

namespace GOCAP.Api.Common;

public class PermissionsControlMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, IPermissionService permissionService)
    {
        // Get current action endpoint
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }
        var permissionAttributes = endpoint?.Metadata.GetOrderedMetadata<RequirePermissionsAttribute>();

        // If not having require permission then skip checking.
        if (permissionAttributes == null || !permissionAttributes.Any())
        {
            await _next(context);
            return;
        }
        var requiredPermissions = permissionAttributes.SelectMany(attr => attr.RequiredPermissions)
                                                      .Distinct()
                                                      .ToList();
        // Get user id from jwt token
        var userId = Guid.Parse("af1da9bb-7f84-4280-8c8c-dac67221c36d");
        var userPermissions = await permissionService.GetUserPermissionsByUserIdAsync(userId);
        var userPermissionTypes = userPermissions.Select(x => x.Type).ToList();

        if (!requiredPermissions.All(userPermissionTypes.Contains))
        {
            throw new AuthenticationFailedException("Forbidden: You do not have the required permissions.");
        }
    }
}
