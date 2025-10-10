using RinhaBackend.Api.Domain.Entity;

namespace RinhaBackend.Api.Domain.Interface;

public interface IRedisService
{
    Task<IEnumerable<Payment>> GetPaymentsAsync(DateTime? from, DateTime? to);
    Task SetPaymentAsync(Payment value, TimeSpan? expiry = null);
}