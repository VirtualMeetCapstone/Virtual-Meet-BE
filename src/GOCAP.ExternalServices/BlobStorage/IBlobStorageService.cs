namespace GOCAP.Services.BlobStorage;

public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="fileStream">The file data stream.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="contentType">The content type of the file (optional).</param>
    /// <param name="maxBlobSize">The max blob size of the file (optional).</param>
    /// <returns>The URL of the uploaded file.</returns>
    Task<List<Media>> UploadFileAsync(List<MediaUpload> mediaUploads);

    /// <summary>
    /// Downloads a file from Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream containing the file data.</returns>
    Task<Stream> DownloadFileAsync(string containerName, string fileName);

    /// <summary>
    /// Deletes a file from Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>True if deletion was successful, otherwise false.</returns>
    Task<bool> DeleteFileAsync(string containerName, string fileName);

    /// <summary>
    /// Checks if a file exists in Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>True if the file exists, otherwise false.</returns>
    Task<bool> FileExistsAsync(string containerName, string fileName);

    /// <summary>
    /// Retrieves a list of all files in a container.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A list of file names.</returns>
    Task<IEnumerable<string>> GetListFilesAsync(string containerName);

    /// <summary>
    /// Generates a time-limited access URL for a file.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="expiryTime">The expiration time for the URL.</param>
    /// <returns>A publicly accessible URL valid for the specified duration.</returns>
    Task<string> GenerateFileUrlAsync(string containerName, string fileName, TimeSpan expiryTime);
}
