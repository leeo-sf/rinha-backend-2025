using RinhaBackend.Api.Contract;

namespace RinhaBackend.Api.Interface;

public interface IRabbitMQService
{
    Task PublisherAsync<T>(RabbitMQCredentials credentials, T data, string queueName);
}