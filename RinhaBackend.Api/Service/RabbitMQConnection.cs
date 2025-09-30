using RabbitMQ.Client;
using RinhaBackend.Api.Config;
using RinhaBackend.Api.Interface;

namespace RinhaBackend.Api.Service;

public class RabbitMQConnection
    : IRabbitMQConnection
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMQConnection(AppConfiguration appConfiguration)
    {
        _factory = new ConnectionFactory
        {
            HostName = appConfiguration.RabbitMQ.HostName,
            UserName = appConfiguration.RabbitMQ.UserName,
            Password = appConfiguration.RabbitMQ.Password,
            VirtualHost = appConfiguration.RabbitMQ.VirtualHost
        };
    }

    public async ValueTask<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        var connection = await CreateConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync();
    }

    public async ValueTask<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        _connection = await _factory.CreateConnectionAsync(cancellationToken);
        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}