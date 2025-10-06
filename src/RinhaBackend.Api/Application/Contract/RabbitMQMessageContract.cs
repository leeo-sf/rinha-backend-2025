namespace RinhaBackend.Api.Application.Contract;

public record RabbitMQMessageContract<T>
    (RabbitMQCredentials credentials, string QueueName, T Message);