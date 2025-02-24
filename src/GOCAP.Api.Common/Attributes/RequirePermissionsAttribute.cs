namespace GOCAP.Api.Common;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionsAttribute(params PermissionType[] permissions) : Attribute
{
    public PermissionType[] RequiredPermissions { get; } = permissions;
}
