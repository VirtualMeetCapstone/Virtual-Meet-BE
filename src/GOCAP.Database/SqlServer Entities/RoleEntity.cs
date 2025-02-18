namespace GOCAP.Database;

[Table("Roles")]
public class RoleEntity : EntitySqlBase
{
    [MaxLength(AppConstants.MaxLengthName)]
    public required string Name { get; set; }

    // Relationships
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
}
