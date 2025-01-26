namespace GOCAP.Api.Model;

public class UserModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Picture { get; set; }
    public string? Bio { get; set; }
    public ICollection<UserPost>? Posts { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
