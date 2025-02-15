namespace GOCAP.Api.Model;

public class StoryCreationModel
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public IFormFile? MediaUpload { get; set; }
    public string? TextContent { get; set; }
    public string? MusicUrl { get; set; }
}
