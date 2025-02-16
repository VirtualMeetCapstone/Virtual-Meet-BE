namespace GOCAP.Database;

[Table("Users")]
public class UserEntity : EntitySqlBase
{
    [MaxLength(GOCAPConstants.MaxLengthName)]
    public required string Name { get; set; } 
    [MaxLength(GOCAPConstants.MaxLengthEmail)]
    public required string Email { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthUrl)]
    public required string Picture { get; set; }
    [MaxLength(GOCAPConstants.MaxLengthDescription)]
    public string? Bio { get; set; }
    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Location { get; set; }
    public UserStatusType? Status { get; set; }
    [Required]
    public bool IsDeleted { get; set; }

    // Relationships
    public ICollection<RoomFavouriteEntity> RoomFavourites { get; set; } = [];
    public ICollection<RoomMemberEntity> RoomMembers { get; set; } = [];
    public ICollection<GroupMemberEntity> GroupMembers { get; set; } = [];
    public ICollection<GroupEntity> Groups { get; set; } = [];
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public ICollection<UserActivityEntity> Activities { get; set; } = [];
    public ICollection<UserFollowEntity> Follows { get; set; } = [];
    public ICollection<PostEntity> Posts { get; set; } = [];
    public ICollection<UserNotificationEntity> Notifications { get; set; } = [];
    public ICollection<UserRewardEntity> Rewards { get; set; } = [];
    public ICollection<UserBlockEntity> Blocks { get; set; } = [];
    public ICollection<StoryEntity> Stories { get; set; } = [];
}
