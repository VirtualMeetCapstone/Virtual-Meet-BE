namespace GOCAP.Database;

[Table("Roles")]
public class RoleEntity : EntitySqlBase
{
    [MaxLength(AppConstants.MaxLengthName)]
    public string Name { get; set; } = string.Empty;
    public RoleType? Type { get; set; }

    // Relationships
    public ICollection<RoleHierarchyEntity> ParentRoles { get; set; } = [];
    public ICollection<RoleHierarchyEntity> ChildRoles { get; set; } = [];
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
}
