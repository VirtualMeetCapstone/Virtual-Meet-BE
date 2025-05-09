﻿using GOCAP.Database.Common.Entities;

namespace GOCAP.Database;

[Table("Users")]
public class UserEntity : EntitySqlBase, ISoftDelete
{
    [MaxLength(AppConstants.MaxLengthName)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthEmail)]
    [Required]
    public string Email { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthUrl)]
    [Required]
    public string Picture { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthDescription)]
    public string? Bio { get; set; }
    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Location { get; set; }
    public UserStatusType? Status { get; set; }
    public long? DeleteTime { get; set; }
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
    public ICollection<UserRewardEntity> Rewards { get; set; } = [];
    public ICollection<UserBlockEntity> Blocks { get; set; } = [];
    public ICollection<StoryEntity> Stories { get; set; } = [];
    public ICollection<StoryHightLightEntity> StoryHightLights { get; set; } = [];
}
