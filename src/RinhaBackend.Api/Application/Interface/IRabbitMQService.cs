using RinhaBackend.Api.Application.Contract;

namespace RinhaBackend.Api.Application.Interface;

public interface IRabbitMQService
{
    Task PublisherAsync<T>(RabbitMQCredentials credentials, T data, string queueName);
}