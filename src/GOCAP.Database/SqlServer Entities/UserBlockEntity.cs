namespace GOCAP.Database;

[Table("UserBlocks")]
public class UserBlockEntity : EntitySqlBase
{
    public Guid BlockedUserId { get; set; }
    public Guid BlockedByUserId { get; set; }
    public UserEntity? BlockedUser { get; set; }
    public UserEntity? BlockedByUser { get; set; }
}
