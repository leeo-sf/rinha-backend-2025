using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace RinhaBackend.Api.Interface;

public interface IRabbitMQConnection
{
    ValueTask<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    ValueTask<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
}