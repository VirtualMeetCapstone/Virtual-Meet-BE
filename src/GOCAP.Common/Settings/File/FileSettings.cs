namespace GOCAP.Common;

public class FileSettings
{
    public List<string> ImageExtensions { get; set; } = [];
    public List<string> VideoExtensions { get; set; } = [];
    public List<string> AudioExtensions { get; set; } = [];
    public List<string> DocumentExtensions { get; set; } = [];
    public List<string> ArchiveExtensions { get; set; } = [];

    public List<string> GetAllValidExtensions()
    {
        return ImageExtensions
            .Concat(VideoExtensions)
            .Concat(AudioExtensions)
            .Concat(DocumentExtensions)
            .Concat(ArchiveExtensions)
            .Select(e => e.ToLower()) 
            .ToList();
    }
}
