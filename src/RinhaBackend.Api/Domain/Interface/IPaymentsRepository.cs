using RinhaBackend.Api.Domain.Entity;

namespace RinhaBackend.Api.Domain.Interface;

public interface IPaymentsRepository
{
    Task<List<Payment>> PaymentsProcessedAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken);
    Task CreatePaymentAsync(List<Payment> payment, CancellationToken cancellationToken);
}