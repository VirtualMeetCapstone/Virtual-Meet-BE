namespace GOCAP.Services.BlobStorage;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string containerName, string fileName);
    Task<Stream> DownloadFileAsync(string containerName, string fileName);
}
