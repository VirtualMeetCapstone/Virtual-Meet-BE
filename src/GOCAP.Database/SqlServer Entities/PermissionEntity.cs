namespace GOCAP.Database;

[Table("Permissions")]
public class PermissionEntity : EntitySqlBase
{
    public required PermissionType Type { get; set; }
    [MaxLength(AppConstants.MaxLengthName)]
    public required string Name { get; set; }
    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
}
