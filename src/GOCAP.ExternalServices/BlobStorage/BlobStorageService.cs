using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace GOCAP.ExternalServices;

public class BlobStorageService(BlobServiceClient _blobServiceClient) : IBlobStorageService
{
    public async Task<Media> UploadFileAsync(MediaUpload mediaUpload)
    {
        if (mediaUpload?.FileStream == null || mediaUpload.FileStream.Length == 0)
        {
            throw new ParameterInvalidException("File stream cannot be null or empty.");
        }

        if (mediaUpload.FileStream.Length > mediaUpload.MaxBlobSize)
        {
            throw new ParameterInvalidException($"File {mediaUpload.FileName} exceeds maximum size of {mediaUpload.MaxBlobSize / (1024 * 1024)} MB.");
        }

        ValidateInput(mediaUpload.ContainerName, mediaUpload.FileName);

        var blobContainerClient = await GetContainerClientAsync(mediaUpload.ContainerName);

        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmm");
        string fileName = $"{Guid.NewGuid()}_{mediaUpload.Type}{timestamp}_{mediaUpload.FileName}";
        string fileNameWithPath = $"{mediaUpload.ContainerName}/{fileName}";
        var blobClient = blobContainerClient.GetBlobClient(fileNameWithPath);

        await blobClient.UploadAsync(mediaUpload.FileStream, overwrite: false);

        return new Media
        {
            Url = blobClient.Uri.ToString(),
            Type = mediaUpload.Type,
            Name = fileName
        };
    }

    public async Task<List<Media>> UploadFilesAsync(List<MediaUpload> mediaUploads)
    {
        if (mediaUploads == null || mediaUploads.Count == 0)
        {
            throw new ParameterInvalidException("File stream cannot be null or empty.");
        }
        var result = new List<Task<Media>>();

        foreach (var mediaUpload in mediaUploads)
        {
            result.Add(Task.Run(async () =>
            {
                if (mediaUpload.FileStream == null || mediaUpload.FileStream.Length == 0)
                {
                    throw new ParameterInvalidException($"File stream cannot be null or empty for {mediaUpload.FileName}");
                }

                if (mediaUpload.FileStream.Length > mediaUpload.MaxBlobSize)
                {
                    throw new ParameterInvalidException($"File {mediaUpload.FileName} exceeds maximum size of {mediaUpload.MaxBlobSize / (1024 * 1024)} MB.");
                }

                ValidateInput(mediaUpload.ContainerName, mediaUpload.FileName);

                var blobContainerClient = await GetContainerClientAsync(mediaUpload.ContainerName);

                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                string fileName = $"{Guid.NewGuid()}_{mediaUpload.Type}{timestamp}_{mediaUpload.FileName}";
                string fileNameWithPath = $"{mediaUpload.ContainerName}/{fileName}";
                var blobClient = blobContainerClient.GetBlobClient(fileNameWithPath);

                await blobClient.UploadAsync(mediaUpload.FileStream, overwrite: false);

                return new Media
                {
                    Url = blobClient.Uri.ToString(),
                    Type = mediaUpload.Type
                };
            }));
        }

        var uploadedFiles = await Task.WhenAll(result);
        return [.. uploadedFiles];
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        ValidateInput(containerName, fileName);
        var blobContainerClient = await GetContainerClientAsync(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync())
        {
            throw new ResourceNotFoundException($"File '{fileName}' does not exist in container '{containerName}'.");
        }
        BlobDownloadInfo download = await blobClient.DownloadAsync();
        return download.Content;
    }

    public async Task<bool> DeleteFilesAsync(List<MediaDelete> mediaDeletes)
    {
        if (mediaDeletes is null || mediaDeletes.Count == 0)
        {
            throw new ParameterInvalidException("Media file to delete can not be null or empty.");
        }

        var deleteTasks = new List<Task<Response<bool>>>();

        foreach (var mediaDelete in mediaDeletes)
        {
            var blobContainerClient = await GetContainerClientAsync(mediaDelete.ContainerName);
            string fileNameWithPath = $"{mediaDelete.ContainerName}/{mediaDelete.FileName}";
            var blobClient = blobContainerClient.GetBlobClient(fileNameWithPath);
            deleteTasks.Add(blobClient.DeleteIfExistsAsync());
        }

        var results = await Task.WhenAll(deleteTasks);

        return results.All(r => r);
    }

    public async Task<bool> DeleteFilesByUrlsAsync(List<string?>? fileUrls)
    {
        if (fileUrls == null || fileUrls.Count == 0)
        {
            throw new ParameterInvalidException("File URLs cannot be null or empty.");
        }

        var deleteTasks = new List<Task<Response<bool>>>();
        foreach (var fileUrl in fileUrls)
        {
            if (fileUrl is null)
            {
                continue;
            }
            var uri = new Uri(fileUrl);
            string[] segments = uri.AbsolutePath.Trim('/').Split('/');

            if (segments.Length < 2)
            {
                throw new ParameterInvalidException($"Invalid Azure Blob Storage URL format: {fileUrl}");
            }

            string containerName = segments[0];
            string blobName = string.Join("/", segments.Skip(1));

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            deleteTasks.Add(blobClient.DeleteIfExistsAsync());
        }

        var results = await Task.WhenAll(deleteTasks);
        return results.All(result => result.Value);
    }

    public async Task<bool> CheckFileExistsAsync(string containerName, string fileName)
    {
        ValidateInput(containerName, fileName);
        var blobContainerClient = await GetContainerClientAsync(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        return await blobClient.ExistsAsync();
    }

    public async Task<IEnumerable<string>> GetListFilesAsync(string containerName)
    {
        if (string.IsNullOrEmpty(containerName))
        {
            throw new ParameterInvalidException("Container name cannot be null or empty.");
        }
        var blobContainerClient = await GetContainerClientAsync(containerName);
        var blobItems = blobContainerClient.GetBlobsAsync();
        var fileList = new List<string>();
        await foreach (var blobItem in blobItems)
        {
            fileList.Add(blobItem.Name);
        }
        return fileList;
    }

    public async Task<string> GenerateFileUrlAsync(string containerName, string fileName, TimeSpan expiryTime)
    {
        ValidateInput(containerName, fileName);
        var blobContainerClient = await GetContainerClientAsync(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            throw new ResourceNotFoundException($"File '{fileName}' does not exist in container '{containerName}'.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
        return $"{blobClient.Uri}{sasToken}";
    }

    public async Task<bool> UpdateFilesAsync(List<MediaUpload> mediaUploads)
    {
        if (mediaUploads == null || mediaUploads.Count == 0)
        {
            throw new ParameterInvalidException("File stream cannot be null or empty.");
        }
        var result = new List<Task>();

        foreach (var mediaUpload in mediaUploads)
        {
            result.Add(Task.Run(async () =>
            {
                if (mediaUpload.FileStream == null || mediaUpload.FileStream.Length == 0)
                {
                    throw new ParameterInvalidException($"File stream cannot be null or empty for {mediaUpload.FileName}");
                }

                if (mediaUpload.FileStream.Length > mediaUpload.MaxBlobSize)
                {
                    throw new ParameterInvalidException($"File {mediaUpload.FileName} exceeds maximum size of {mediaUpload.MaxBlobSize / (1024 * 1024)} MB.");
                }

                ValidateInput(mediaUpload.ContainerName, mediaUpload.FileName);

                var blobContainerClient = await GetContainerClientAsync(mediaUpload.ContainerName);
                string fileNameWithPath = $"{mediaUpload.ContainerName}/{mediaUpload.FileName}";
                var blobClient = blobContainerClient.GetBlobClient(fileNameWithPath);

                if (!await blobClient.ExistsAsync())
                {
                    throw new ResourceNotFoundException($"File '{mediaUpload.FileName}' does not exist in container '{mediaUpload.ContainerName}'.");
                }

                await blobClient.UploadAsync(mediaUpload.FileStream, overwrite: true);
            }));
        }
        try
        {
            await Task.WhenAll(result);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync();
        return blobContainerClient;
    }

    private static void ValidateInput(string containerName, string fileName)
    {
        if (string.IsNullOrEmpty(containerName))
        {
            throw new ParameterInvalidException("Container name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ParameterInvalidException("File name cannot be null or empty.");
        }
    }

    
}
