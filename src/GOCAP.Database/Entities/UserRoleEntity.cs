using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("UserRoles")]
public class UserRoleEntity : EntityBase
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    public Guid RoleId { get; set; }
    public RoleEntity? Role { get; set; }
}
