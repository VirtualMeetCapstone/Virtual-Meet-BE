namespace GOCAP.Domain;

public class SearchHistory : DateTrackingBase
{
    public Guid? UserId { get; set; }
    public string Query { get; set; } = string.Empty;
}
