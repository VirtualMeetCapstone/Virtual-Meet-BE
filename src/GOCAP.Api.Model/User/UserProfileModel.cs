namespace GOCAP.Api.Model;

public class UserProfileModel
{
    public string? Name { get; set; }
    public Media? Picture { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount{ get; set; }
    public int FriendsCount { get; set; }
    public UserStatusType? Status { get; set; }

}
