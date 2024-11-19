namespace DocumentsREST.BL.Services;

public interface IRabbitMqService
{
    Task PublishMessageAsync(string message);
    Task InitializeQueueAsync(string queueName);
}
