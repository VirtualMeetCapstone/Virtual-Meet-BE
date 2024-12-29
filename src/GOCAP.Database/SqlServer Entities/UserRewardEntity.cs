namespace GOCAP.Database;

[Table("UserRewards")]
public class UserRewardEntity : EntitySqlBase
{
    public Guid UserId { get; set; }
    public string? RewardType { get; set; }
    public DateTime RewardDate { get; set; }
    public UserEntity? User { get; set; }
}
