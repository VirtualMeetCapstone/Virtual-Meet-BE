namespace GOCAP.Domain;
public class User : CreatedObjectBase, ISoftDelete
{
    public string? Email { get; set; }
    public Media? Picture { get; set; }
    public MediaUpload? PictureUpload { get; set; }
    public string? Locale { get; set; }
    public string? Bio { get; set; }

    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }
    public string? Gender { get; set; }
    public string? Birthday { get; set; }
    public string? Location { get; set; }
    public UserStatusType? Status { get; set; }
    public int FollowersCount { get; set; } 
    public int FollowingsCount { get; set; }
    public int FriendsCount { get; set; }
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<Follow> Followers { get; set; } = [];
    public ICollection<Follow> Followings { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public bool IsDeleted { get; set;}
    public long? DeleteTime { get; set;}
}
