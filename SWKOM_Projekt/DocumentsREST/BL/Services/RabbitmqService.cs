using System.Text;
using RabbitMQ.Client;
namespace DocumentsREST.BL.Services;

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _queueName;

    public RabbitMqService(string queueName, string hostname = "localhost", string username = "guest", string password = "guest")
    {
        _queueName = queueName;

        var factory = new ConnectionFactory
        {
            HostName = hostname,
            UserName = username,
            Password = password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        InitializeQueueAsync(_queueName).Wait();
    }

    public async Task InitializeQueueAsync(string queueName)
    {
        _channel.QueueDeclare(queueName, false, false, false, null);
        await Task.CompletedTask; // RabbitMQ's queue creation is synchronous.
    }

    public async Task PublishMessageAsync(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}