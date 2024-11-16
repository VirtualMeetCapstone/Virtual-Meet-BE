using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("Roles")]
public class RoleEntity : EntityBase
{
    [Required]
    [StringLength(50)]
    public string? Name { get; set; }

    // Relationships
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
}
