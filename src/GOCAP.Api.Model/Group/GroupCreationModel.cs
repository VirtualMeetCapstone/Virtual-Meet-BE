namespace GOCAP.Api.Model;

public class GroupCreationModel
{
    public Guid OwnerId { get; set; }
    public string? Picture { get; set; }
    public string? GroupName { get; set; }
}
