using Microsoft.EntityFrameworkCore;
using RinhaBackend.Api.Data.Entity;
using RinhaBackend.Api.Interface;

namespace RinhaBackend.Api.Data.Repository;

public class PaymentsRepository
    : IPaymentsRepository
{
    private readonly AppDbContext _appDbContext;

    public PaymentsRepository(AppDbContext appDbContext)
        => _appDbContext = appDbContext;

    public async Task<List<Payment>> PaymentsProcessedAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
        => await _appDbContext.Payments
            .AsNoTracking()
            .Where(p => p.RequestedAt >= from && p.RequestedAt <= to
            && p.IsProcessed)
            .ToListAsync(cancellationToken);

    public async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePaymentToProcessed(Payment payment, CancellationToken cancellationToken)
    {
        _appDbContext.Payments.Update(payment);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}