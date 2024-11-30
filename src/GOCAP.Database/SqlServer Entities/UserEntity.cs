namespace GOCAP.Database;

[Table("Users")]
public class UserEntity : EntitySqlBase
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public string? Picture { get; set; }
    public string? Bio { get; set; }
    public bool? VerifiedEmail { get; set; }
    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Location { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }

    // Relationships
    public ICollection<PostEntity> Posts { get; set; } = [];
    //public ICollection<FollowEntity> Follows { get; set; } = [];
    public ICollection<NotificationEntity> Notifications { get; set; } = [];
    public ICollection<RoleEntity> Roles { get; set; } = [];
}
