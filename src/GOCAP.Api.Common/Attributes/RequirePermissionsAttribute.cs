namespace GOCAP.Api.Common;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionsAttribute(params PermissionType[] permissions) : Attribute
{
    public PermissionType[] RequiredPermissions { get; } = permissions;
}
