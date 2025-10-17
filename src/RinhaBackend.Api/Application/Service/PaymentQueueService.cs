using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Interface;
using System.Threading.Channels;

namespace RinhaBackend.Api.Application.Service;

public class PaymentQueueService : IPaymentQueueService
{
    private const int MaxQueueSize = 8000;
    private readonly Channel<PaymentContract> _channel;

    public PaymentQueueService()
    {
        var options = new BoundedChannelOptions(MaxQueueSize)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropNewest
        };
        _channel = Channel.CreateBounded<PaymentContract>(options);
    }

    public async ValueTask EnqueueAsync(PaymentContract request, CancellationToken cancellationToken = default)
        => await _channel.Writer.WriteAsync(request, cancellationToken);

    public IAsyncEnumerable<PaymentContract> ReadAllAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}