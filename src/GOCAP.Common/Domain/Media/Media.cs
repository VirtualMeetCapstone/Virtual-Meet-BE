namespace GOCAP.Common;

public class Media
{
    public string? Url { get; set; }
    public MediaType Type { get; set; }
    public string? ThumbnailUrl { get; set; } = null;
    public string? Name { get; set; }
}