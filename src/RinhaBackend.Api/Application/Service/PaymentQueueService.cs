using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Interface;
using System.Threading.Channels;

namespace RinhaBackend.Api.Application.Service;

public class PaymentQueueService : IPaymentQueueService
{
    private readonly Channel<PaymentContract> _channel;

    public PaymentQueueService()
        => _channel = Channel.CreateUnbounded<PaymentContract>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(PaymentContract request, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(request, cancellationToken);

    public IAsyncEnumerable<PaymentContract> ReadAllASync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}