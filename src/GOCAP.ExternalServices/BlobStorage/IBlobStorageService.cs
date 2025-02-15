namespace GOCAP.ExternalServices;

public interface IBlobStorageService
{
    /// <summary>
    /// Upload a file to Azure Blob Storage.
    /// </summary>
    /// <param name="mediaUpload"></param>
    /// <returns>Media</returns>
    Task<Media> UploadFileAsync(MediaUpload mediaUpload);

    /// <summary>
    /// Upload many files to Azure Blob Storage.
    /// </summary>
    /// <param name="mediaUploads">List<MediaUpload></param>
    /// <returns>The list media</returns>
    Task<List<Media>> UploadFilesAsync(List<MediaUpload> mediaUploads);

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
    Task<bool> DeleteFilesAsync(List<MediaDelete> mediaDeletes);

    /// <summary>
    /// Checks if a file exists in Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>True if the file exists, otherwise false.</returns>
    Task<bool> CheckFileExistsAsync(string containerName, string fileName);

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
    Task<bool> DeleteFilesByUrlsAsync(List<string?>? fileUrls);
    Task<bool> UpdateFilesAsync(List<MediaUpload> mediaUploads);
}
