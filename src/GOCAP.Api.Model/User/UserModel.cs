namespace GOCAP.Api.Model;

public class UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }
    public int FriendsCount { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
