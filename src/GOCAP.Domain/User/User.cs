namespace GOCAP.Domain;

public class User : CreatedObjectBase
{
    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Picture { get; set; }
    public string? Locale { get; set; }
    public string? Bio { get; set; }

    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }
    public bool? VerifiedEmail { get; set; }

    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Hometown { get; set; }
    public string? Location { get; set; }

    // Relationships
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<Follow> Followers { get; set; } = [];
    public ICollection<Follow> Following { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
