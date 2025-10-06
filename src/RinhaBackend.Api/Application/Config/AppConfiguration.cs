using RinhaBackend.Api.Application.Contract;

namespace RinhaBackend.Api.Application.Config;

public class AppConfiguration
{
    public RabbitMQCredentials RabbitMQ { get; init; } = default!;
}