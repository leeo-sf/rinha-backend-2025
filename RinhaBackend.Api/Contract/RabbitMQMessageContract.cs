namespace RinhaBackend.Api.Contract;

public record RabbitMQMessageContract<T>
    (RabbitMQCredentials credentials, string QueueName, T Message);