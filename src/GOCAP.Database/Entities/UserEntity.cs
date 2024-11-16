using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("Users")]
public class UserEntity : UserCreatedObject
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Picture { get; set; }
    public string? Locale { get; set; }
    public string? Hd { get; set; } // Domain from Google
    public string? Bio { get; set; }

    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }
    public bool? VerifiedEmail { get; set; }

    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Hometown { get; set; }
    public string? Location { get; set; }

    // Relationships
    public ICollection<PostEntity> Posts { get; set; } = [];
    public ICollection<FollowEntity> Followers { get; set; } = [];
    public ICollection<FollowEntity> Following { get; set; } = [];
    public ICollection<NotificationEntity> Notifications { get; set; } = [];
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
}
