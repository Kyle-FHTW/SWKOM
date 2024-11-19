namespace DocumentsREST.BL.Services;

public interface IMinioService
{
    Task EnsureBucketExistsAsync(string bucketName);
    Task UploadFileAsync(string fileName, Stream data, long fileSize, string contentType);
    Task<Stream> DownloadFileAsync(string fileName); // New method for downloading files
}