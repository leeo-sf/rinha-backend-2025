using RinhaBackend.Api.Data.Entity;
using RinhaBackend.Api.Interface;

namespace RinhaBackend.Api.Data.Repository;

public class PaymentsRepository
    : IPaymentsRepository
{
    private readonly AppDbContext _appDbContext;

    public PaymentsRepository(AppDbContext appDbContext)
        => _appDbContext = appDbContext;

    public async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}