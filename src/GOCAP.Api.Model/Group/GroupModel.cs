namespace GOCAP.Api.Model;

public class GroupModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public required string Name { get; set; } 
    public string? Picture { get; set; }
}