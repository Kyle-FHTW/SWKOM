using Minio;
using Minio.DataModel.Args;
using System.IO;
using System.Threading.Tasks;

namespace DocumentsREST.BL.Services
{
    public class MinioService : IMinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;

        public MinioService(string endpoint, string accessKey, string secretKey, string bucketName, bool useSSL = false)
        {
            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(useSSL)
                .Build();

            _bucketName = bucketName;

            EnsureBucketExistsAsync(_bucketName).Wait();
        }

        public async Task EnsureBucketExistsAsync(string bucketName)
        {
            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucketName)
            );

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucketName)
                );
            }
        }

        public async Task UploadFileAsync(string fileName, Stream data, long fileSize, string contentType)
        {
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(data)
                .WithObjectSize(fileSize)
                .WithContentType(contentType));
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            fileName += ".pdf";

            var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        
        public async Task DeleteFileAsync(string fileName)
        {
            fileName += ".pdf";

            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName));
        }
    }
}
