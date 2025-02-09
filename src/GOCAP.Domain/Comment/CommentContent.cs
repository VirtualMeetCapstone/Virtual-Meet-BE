namespace GOCAP.Domain;

public class CommentContent
{
    public required MediaType Type { get; set; }
    public required string Value { get; set; }
    public string? Thumbnail { get; set; } = null;
}
