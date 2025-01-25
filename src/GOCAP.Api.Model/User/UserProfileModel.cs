namespace GOCAP.Api.Model;

public class UserProfileModel
{
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount{ get; set; }
    public ICollection<UserPost> Posts { get; set; } = [];
}
