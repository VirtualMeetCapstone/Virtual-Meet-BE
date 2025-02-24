namespace GOCAP.Api.Model;

public class SearchHistoryCreationModel
{
    public Guid? UserId { get; set; }
    public string Query { get; set; } = string.Empty;
}
