using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using ImageMagick;
using Tesseract;
using System.Diagnostics;

namespace OCRWorker
{
    public class OcrWorker
    {
        private IConnection _connection;
        private IModel _channel;

        public OcrWorker()
        {
            ConnectToRabbitMQ();
        }

        private void ConnectToRabbitMQ()
        {
            int retries = 5;
            while (retries > 0)
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "DocumentsRabbitMQ", UserName = "admin", Password = "admin" };
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _channel.QueueDeclare(queue: "document_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    _channel.QueueDeclare(queue: "ocr_result_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    Console.WriteLine("Successfully connected to RabbitMQ and queues declared.");

                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}. Retrying in 5 seconds...");
                    Thread.Sleep(5000);
                    retries--;
                }
            }

            if (_connection == null || !_connection.IsOpen)
            {
                throw new Exception("Failed to connect to RabbitMQ after multiple attempts.");
            }
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var parts = message.Split('|');

                if (parts.Length == 2)
                {
                    var id = parts[0];
                    var filePath = parts[1];

                    Console.WriteLine($"[x] Received ID: {id}, FilePath: {filePath}");

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"Error: File {filePath} not found.");
                        return;
                    }

                    var extractedText = PerformOcr(filePath);

                    if (!string.IsNullOrEmpty(extractedText))
                    {
                        var resultBody = Encoding.UTF8.GetBytes($"{id}|{extractedText}");
                        _channel.BasicPublish(exchange: "", routingKey: "ocr_result_queue", basicProperties: null, body: resultBody);

                        Console.WriteLine($"[x] Sent result for ID: {id}");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Invalid message received, split into less than 2 parts.");
                }
            };

            _channel.BasicConsume(queue: "document_queue", autoAck: true, consumer: consumer);
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
                            string result = process.StandardOutput.ReadToEnd();
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

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}