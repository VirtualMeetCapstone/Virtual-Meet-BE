namespace GOCAP.Database;

[Table("UserActivities")]
public class UserActivityEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    public string? ActivityType { get; set; }
    public DateTime ActivityTime { get; set; }
    public UserEntity? User { get; set; }
}
