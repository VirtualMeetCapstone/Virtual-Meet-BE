namespace GOCAP.Database;

[Table("UserRoles")]
public class UserRoleEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public UserEntity? User { get; set; }
    public RoleEntity? Role { get; set; }
}
