namespace GOCAP.Api.Validation;

public class FileUploadValidator (IAppConfiguration _appConfiguration)
{
    private readonly FileSettings _fileSettings = _appConfiguration.GetFileSettings();
    public bool IsValidFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return _fileSettings.GetAllValidExtensions().Contains(extension);
    }

    public bool IsValidImage(string fileName) => _fileSettings.ImageExtensions.Contains(Path.GetExtension(fileName).ToLower());
    public bool IsValidVideo(string fileName) => _fileSettings.VideoExtensions.Contains(Path.GetExtension(fileName).ToLower());
    public bool IsValidAudio(string fileName) => _fileSettings.AudioExtensions.Contains(Path.GetExtension(fileName).ToLower());
    public bool IsValidDocument(string fileName) => _fileSettings.DocumentExtensions.Contains(Path.GetExtension(fileName).ToLower());
    public bool IsValidArchive(string fileName) => _fileSettings.ArchiveExtensions.Contains(Path.GetExtension(fileName).ToLower());

    public bool IsValidNumberOfMediaFile(ICollection<IFormFile>? files)
    {
        if (files == null || files.Count == 0)
        {
            return false;
        }

        int imageCount = 0;
        int videoCount = 0;

        foreach (var file in files)
        {
            // Get tail file
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (_fileSettings.ImageExtensions.Contains(extension))
            {
                imageCount++;
            }
            else if (_fileSettings.VideoExtensions.Contains(extension))
            {
                videoCount++;
            }
            else
            {
                return false;
            }
        }

        return (imageCount >= 1 && imageCount <= 5 && videoCount == 0) || (videoCount == 1 && imageCount == 0);
    }
}
