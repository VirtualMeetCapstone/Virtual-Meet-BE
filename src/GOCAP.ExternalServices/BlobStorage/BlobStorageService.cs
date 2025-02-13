using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace GOCAP.Services.BlobStorage;

public class BlobStorageService(BlobServiceClient _blobServiceClient) : IBlobStorageService
{
    public async Task<List<Media>> UploadFileAsync(List<MediaUpload> mediaUploads)
    {
        if (mediaUploads == null || mediaUploads.Count == 0)
        {
            throw new ParameterInvalidException("File stream cannot be null or empty.");
        }
        var result = new List<Media>();
        foreach (var mediaUpload in mediaUploads)
        {
            var blobContainerClient = await GetContainerClientAsync(mediaUpload.ContainerName);

            if (mediaUpload.FileStream == null || mediaUpload.FileStream.Length == 0)
            {
                throw new ParameterInvalidException($"File stream cannot be null or empty for {mediaUpload.FileName}");
            }

            if (mediaUpload.FileStream.Length > mediaUpload.MaxBlobSize)
            {
                throw new ParameterInvalidException($"File {mediaUpload.FileName} exceeds maximum size of {mediaUpload.MaxBlobSize / (1024 * 1024)} MB.");
            }

            ValidateInput(mediaUpload.ContainerName, mediaUpload.FileName);

            string fileNameWithPath = $"{mediaUpload.ContainerName}/{mediaUpload.FileName}";
            var blobClient = blobContainerClient.GetBlobClient(fileNameWithPath);

            await blobClient.UploadAsync(mediaUpload.FileStream, overwrite: true);
            result.Add(new Media
            {
                Url = blobClient.Uri.ToString(),
                Type = mediaUpload.Type,
            });
        }
        return result;
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

    public async Task<bool> DeleteFileAsync(string containerName, string fileName)
    {
        ValidateInput(containerName, fileName);
        var blobContainerClient = await GetContainerClientAsync(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<bool> FileExistsAsync(string containerName, string fileName)
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
