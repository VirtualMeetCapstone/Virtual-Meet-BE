namespace GOCAP.Database;

[Table("Users")]
public class UserEntity : EntitySqlBase
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public string? Bio { get; set; }
    public bool? VerifiedEmail { get; set; }
    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Location { get; set; }
    [Required]
    public long CreateTime { get; set; }
    [Required]
    public long LastModifyTime { get; set; }

    // Relationships
    public IEnumerable<RoomEntity> Rooms { get; set; } = [];
    public IEnumerable<GroupEntity> Groups { get; set; } = [];
    public IEnumerable<UserRoleEntity> UserRoles { get; set; } = [];
    public IEnumerable<UserActivityEntity> UserActivities { get; set; } = [];
    public IEnumerable<UserFollowEntity> UserFollows { get; set; } = [];
    public IEnumerable<UserPostEntity> UserPosts { get; set; } = [];
    public IEnumerable<UserNotificationEntity> UserNotifications { get; set; } = [];
    public IEnumerable<UserRewardEntity> UserRewards { get; set; } = [];
    public IEnumerable<UserBlockEntity> UserBlocks { get; set; } = [];
    public IEnumerable<UserStoryEntity> UserStories { get; set; } = [];
}
