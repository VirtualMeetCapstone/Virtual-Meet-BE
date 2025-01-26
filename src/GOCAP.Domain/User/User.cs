namespace GOCAP.Domain;

public class User : CreatedObjectBase
{
    public string? Email { get; set; }
    public string? Picture { get; set; }
    public string? Locale { get; set; }
    public string? Bio { get; set; }

    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }
    public bool? VerifiedEmail { get; set; }

    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Location { get; set; }
    public UserStatusType? Status { get; set; }
    public int FollowersCount { get; set; } 
    public int FollowingsCount { get; set; } 
    public ICollection<UserPost> Posts { get; set; } = [];
    public ICollection<Follow> Followers { get; set; } = [];
    public ICollection<Follow> Followings { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
