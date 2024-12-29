namespace GOCAP.Database;

[Table("UserRoles")]
public class UserRoleEntity : EntitySqlBase
{
    [Required]
    [StringLength(50)]
    public string? Name { get; set; }

    // Relationships
    public IEnumerable<UserEntity> Users { get; set; } = [];
}
