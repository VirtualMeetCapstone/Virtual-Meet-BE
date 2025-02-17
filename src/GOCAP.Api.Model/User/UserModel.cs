namespace GOCAP.Api.Model;

public class UserModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Media? Picture { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }
    public int FriendsCount { get; set; }
}
