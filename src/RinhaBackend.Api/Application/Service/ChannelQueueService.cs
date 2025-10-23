using RinhaBackend.Api.Application.Interface;
using System.Threading.Channels;

namespace RinhaBackend.Api.Application.Service;

public class ChannelQueueService<T> : IChannelQueueService<T>
{
    private const int MaxQueueSize = 16000;
    private readonly Channel<T> _channel;

    public ChannelQueueService()
    {
        var options = new BoundedChannelOptions(MaxQueueSize)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropNewest
        };
        _channel = Channel.CreateBounded<T>(options);
    }

    public async ValueTask EnqueueAsync(T request, CancellationToken cancellationToken = default)
        => await _channel.Writer.WriteAsync(request, cancellationToken);

    public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}