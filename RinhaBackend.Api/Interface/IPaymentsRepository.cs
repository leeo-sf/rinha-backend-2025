using RinhaBackend.Api.Data.Entity;

namespace RinhaBackend.Api.Interface;

public interface IPaymentsRepository
{
    Task<List<Payment>> PaymentsProcessedAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken);
    Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);
    Task UpdatePaymentToProcessed(Payment payment, CancellationToken cancellationToken);
}