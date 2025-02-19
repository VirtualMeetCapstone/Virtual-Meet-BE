namespace GOCAP.Database;

[Table("RolePermissions")]
public class RolePermissionEntity : EntitySqlBase
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public RoleEntity? Role { get; set; }
    public PermissionEntity? Permission { get; set; }
}
