namespace GOCAP.Services.BlobStorage;

public class BlobStorageService(BlobServiceClient _blobServiceClient) : IBlobStorageService
{
    private const long MaxBlobSize = 1024 * 1024 * 50;
    public async Task<string> UploadFileAsync(Stream fileStream, string containerName, string fileName)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream cannot be null or empty", nameof(fileStream));
        }

        if (fileStream.Length > MaxBlobSize)
        {
            throw new InvalidOperationException($"File exceeds maximum size of {MaxBlobSize / (1024 * 1024)}MB.");
        }

        ValidateInput(containerName, fileName);

        try
        {
            // Take Blob Container Client
            var blobContainerClient = await GetContainerClientAsync(containerName);

            // Take blob client base on file name
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            // Upload file to blob, if existed the overwrite
            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 413)
        {
            // Error case when capacity exceeds limit
            throw new InvalidOperationException("Cannot upload because the capacity has exceeded the allowed limit.", ex);
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 507)
        {
            // Error case when storage is not enough    
            throw new InvalidOperationException("Cannot upload because there is not enough space in blob storage.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while uploading the file.", ex);
        }
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        ValidateInput(containerName, fileName);
        try
        {
            // Take Blob Container Client
            var blobContainerClient = await GetContainerClientAsync(containerName);

            // Take Blob Client
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"File '{fileName}' does not exist in container '{containerName}'.");
            }

            // Download and return stream
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            return download.Content;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while uploading the file.", ex);
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
            throw new ArgumentNullException(nameof(containerName), "Container name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
        }
    }
}
