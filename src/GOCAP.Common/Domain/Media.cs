namespace GOCAP.Common;

public class Media
{
    public string? Url { get; set; }
    public MediaType Type { get; set; }
    public string? ThumbnailUrl { get; set; } = null;
}

public class MediaUpload
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = GOCAPConstants.BlobContainer;
    public string? ContentType {  get; set; }
    public long MaxBlobSize { get; set; } = GOCAPConstants.MaxBlobSize;
    public MediaType Type { get; set; }
}

public class MediaDelete
{
    public string ContainerName { get; set; } = "";
    public List<string> FileNames { get; set; } = [];
}