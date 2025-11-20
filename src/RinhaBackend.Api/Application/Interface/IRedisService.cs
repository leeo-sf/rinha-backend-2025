using RinhaBackend.Api.Domain.Entity;

namespace RinhaBackend.Api.Application.Interface;

public interface IRedisService
{
    Task AddPaymentAsync(Payment value);
    Task<List<Payment>?> GetPaymentsAsync(DateTime? start = null, DateTime? end = null);
}