using RinhaBackend.Api.Data.Entity;

namespace RinhaBackend.Api.Interface;

public interface IPaymentsRepository
{
    Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);
}