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
        var userId = Guid.Parse("132550D4-998F-4ADB-A546-330AA5AB78E4");
        var userPermissions = await permissionService.GetUserPermissionsByUserIdAsync(userId);
        var userPermissionTypes = userPermissions.Select(x => x.Type).ToList();

        if (!requiredPermissions.All(userPermissionTypes.Contains))
        {
            throw new ForbiddenException("You do not have the required permissions.");
        }
        await _next(context);
    }
}
