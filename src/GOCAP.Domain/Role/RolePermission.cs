namespace GOCAP.Domain;

public class RolePermission : DateTrackingBase
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
