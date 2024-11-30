namespace GOCAP.Database;

[Table("Roles")]
public class RoleEntity : EntitySqlBase
{
    [Required]
    [StringLength(50)]
    public string? Name { get; set; }

    // Relationships
    public ICollection<UserEntity>? Users { get; set; }
}
