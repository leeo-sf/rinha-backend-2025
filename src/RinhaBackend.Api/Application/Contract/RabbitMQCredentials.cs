namespace RinhaBackend.Api.Application.Contract;

public class RabbitMQCredentials
{
    public string HostName { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string UserName { get; init; } = default!;
    public string VirtualHost { get; init; } = default!;
    public RabbitMQQueueContract Queues { get; init; } = default!;
}