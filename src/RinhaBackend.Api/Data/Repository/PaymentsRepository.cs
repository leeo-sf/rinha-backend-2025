using Microsoft.EntityFrameworkCore;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Interface;

namespace RinhaBackend.Api.Data.Repository;

public class PaymentsRepository
    : IPaymentsRepository
{
    private readonly AppDbContext _appDbContext;

    public PaymentsRepository(AppDbContext appDbContext)
        => _appDbContext = appDbContext;

    public async Task<List<Payment>> PaymentsProcessedAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken)
        => await _appDbContext.Payments
            .AsNoTracking()
            .Where(p => from.HasValue && to.HasValue ? p.RequestedAt >= from && p.RequestedAt <= to : true)
            .ToListAsync(cancellationToken);

    public async Task CreatePaymentAsync(List<Payment> payment, CancellationToken cancellationToken)
    {
        await _appDbContext.Payments.AddRangeAsync(payment);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}