using RinhaBackend.Api.Contract;

namespace RinhaBackend.Api.Config;

public class AppConfiguration
{
    public RabbitMQCredentials RabbitMQ { get; init; } = default!;
}