namespace GOCAP.Api.Model;

public class StoryViewerModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Media? Picture { get; set; }
}
