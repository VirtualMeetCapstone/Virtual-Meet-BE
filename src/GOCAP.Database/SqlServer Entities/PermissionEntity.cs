namespace GOCAP.Database;

[Table("Permissions")]
public class PermissionEntity : EntitySqlBase
{
    public PermissionType Type { get; set; }
    [MaxLength(AppConstants.MaxLengthName)]
    public string Name { get; set; } = string.Empty;
    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
}
