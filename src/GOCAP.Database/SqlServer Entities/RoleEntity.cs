namespace GOCAP.Database;

[Table("Roles")]
public class RoleEntity : EntitySqlBase
{
    [Required]
    [MaxLength(GOCAPConstants.MaxLengthName)]
    public string? Name { get; set; }

    // Relationships
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
}
