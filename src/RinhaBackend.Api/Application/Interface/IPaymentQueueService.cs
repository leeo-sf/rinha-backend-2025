using RinhaBackend.Api.Application.Contract;

namespace RinhaBackend.Api.Application.Interface;

public interface IPaymentQueueService
{
    ValueTask EnqueueAsync(PaymentContract request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PaymentContract> ReadAllASync(CancellationToken cancellationToken = default);
}