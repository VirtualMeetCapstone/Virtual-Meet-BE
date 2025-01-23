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
    public UserStatusType? Status { get; set; }
    [Required]
    public long CreateTime { get; set; }
    [Required]
    public long LastModifyTime { get; set; }

    // Relationships
    public ICollection<RoomFavouriteEntity> RoomFavourites { get; set; } = [];
    public ICollection<GroupEntity> Groups { get; set; } = [];
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public ICollection<UserActivityEntity> Activities { get; set; } = [];
    public ICollection<UserFollowEntity> Follows { get; set; } = [];
    public ICollection<UserPostEntity> Posts { get; set; } = [];
    public ICollection<UserNotificationEntity> Notifications { get; set; } = [];
    public ICollection<UserRewardEntity> Rewards { get; set; } = [];
    public ICollection<UserBlockEntity> Blocks { get; set; } = [];
    public ICollection<UserStoryEntity> Stories { get; set; } = [];
}
