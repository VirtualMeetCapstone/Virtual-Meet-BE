namespace GOCAP.Database;

[Table("Groups")]
public class GroupEntity : EntitySqlBase
{
    public string? GroupName { get; set; }
    public IEnumerable<UserEntity> Members { get; set; } = [];
}
