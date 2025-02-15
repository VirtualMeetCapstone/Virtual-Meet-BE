namespace GOCAP.Common;

public class MediaUpload
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = GOCAPConstants.BlobContainer;
    public string? ContentType { get; set; }
    public long MaxBlobSize { get; set; } = GOCAPConstants.MaxBlobSize;
    public MediaType Type { get; set; }
}
