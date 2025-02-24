namespace GOCAP.Api.Model;

public class SearchHistoryModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Query { get; set; } = string.Empty;
}
