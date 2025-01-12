public interface IMinioService
{
    Task UploadFileAsync(string objectName, Stream fileStream, long fileSize, string contentType);
    Task<Stream> DownloadFileAsync(string objectName);
    
    // New method to remove a file from MinIO
    Task DeleteFileAsync(string objectName);
}