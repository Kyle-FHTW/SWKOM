#region

using System.Diagnostics;
using System.Text;
using ImageMagick;
using Minio;
using Minio.DataModel.Args;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

#endregion

namespace OCRWorker;

public class OcrWorker : IDisposable
{
    private const string MinioBucketName = "documents";
    private readonly IMinioClient _minioClient;
    private IModel _channel;
    private IConnection _connection;

    public OcrWorker()
    {
        ConnectToRabbitMQ();
        _minioClient = new MinioClient()
            .WithEndpoint("minio", 9000) // MinIO server address and port
            .WithCredentials("minioadmin", "minioadmin") // Replace with your MinIO credentials
            .WithSSL(false) // Adjust based on your setup
            .Build();
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }

    private void ConnectToRabbitMQ()
    {
        var retries = 5;
        while (retries > 0)
            try
            {
                var factory = new ConnectionFactory
                    { HostName = "DocumentsRabbitMQ", UserName = "admin", Password = "admin" };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare("document_queue", false, false, false, null);
                _channel.QueueDeclare("ocr_result_queue", false, false, false, null);
                Console.WriteLine("Successfully connected to RabbitMQ and queues declared.");

                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}. Retrying in 5 seconds...");
                Thread.Sleep(5000);
                retries--;
            }

        if (_connection == null || !_connection.IsOpen)
            throw new Exception("Failed to connect to RabbitMQ after multiple attempts.");
    }

    public void Start()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var parts = message.Split('|');

            if (parts.Length == 2)
            {
                var id = parts[0];
                var minioObjectKey = parts[1];

                Console.WriteLine($"[x] Received ID: {id}, MinIO Object Key: {minioObjectKey}");

                var tempFilePath = await DownloadFileFromMinIO(minioObjectKey);

                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    var extractedText = PerformOcr(tempFilePath);
                    File.Delete(tempFilePath); // Clean up the temporary file

                    if (!string.IsNullOrEmpty(extractedText))
                    {
                        var resultBody = Encoding.UTF8.GetBytes($"{id}|{extractedText}");
                        _channel.BasicPublish("", "ocr_result_queue", null, resultBody);

                        Console.WriteLine($"[x] Sent result for ID: {id}");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Temporary file for OCR processing could not be created.");
                }
            }
            else
            {
                Console.WriteLine("Error: Invalid message received, split into less than 2 parts.");
            }
        };

        _channel.BasicConsume("document_queue", true, consumer);
    }

    private async Task<string> DownloadFileFromMinIO(string objectKey)
    {
        try
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(MinioBucketName)
                .WithObject(objectKey)
                .WithCallbackStream(stream =>
                {
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                }));

            Console.WriteLine($"[x] Successfully downloaded file: {objectKey} to {tempFilePath}");
            return tempFilePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading file from MinIO: {ex.Message}");
            return null;
        }
    }

    private string PerformOcr(string filePath)
    {
        var stringBuilder = new StringBuilder();

        try
        {
            using (var images = new MagickImageCollection(filePath))
            {
                foreach (var image in images)
                {
                    var tempPngFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");

                    image.Density = new Density(300, 300);
                    image.Format = MagickFormat.Png;
                    image.Write(tempPngFile);

                    var psi = new ProcessStartInfo
                    {
                        FileName = "tesseract",
                        Arguments = $"{tempPngFile} stdout -l eng",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        var result = process.StandardOutput.ReadToEnd();
                        stringBuilder.Append(result);
                    }

                    File.Delete(tempPngFile);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during OCR processing: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Stacktrace: {ex.StackTrace}");
            }
        }

        return stringBuilder.ToString();
    }
}