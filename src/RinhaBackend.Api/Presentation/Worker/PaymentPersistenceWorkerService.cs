using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Interface;

namespace RinhaBackend.Api.Presentation.Worker;

public class PaymentPersistenceWorkerService : BackgroundService
{
    private readonly IChannelQueueService<Payment> _queue;
    private readonly ILogger<PaymentPersistenceWorkerService> _logger;
    private readonly IServiceScopeFactory _serviceProvider;
    private const int BATCH_SIZE = 100;
    private const int DELAY_BETWEEN_BATCHES_MS = 10;
    private const int MAX_RETRIES = 3;

    public PaymentPersistenceWorkerService(
        IChannelQueueService<Payment> queue,
        ILogger<PaymentPersistenceWorkerService> logger,
        IServiceScopeFactory serviceProvider)
    {
        _queue = queue;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<Payment>(BATCH_SIZE);
        var lastFlush = DateTime.UtcNow;

        await foreach (var payment in _queue.ReadAllAsync(stoppingToken))
        {
            batch.Add(payment);

            var elapsed = (DateTime.UtcNow - lastFlush).TotalMilliseconds;
            if (batch.Count >= BATCH_SIZE || elapsed >= DELAY_BETWEEN_BATCHES_MS)
            {
                await PersistBatchAsync(batch, stoppingToken);
                batch.Clear();
                lastFlush = DateTime.UtcNow;
            }
        }

        if (batch.Count > 0)
            await PersistBatchAsync(batch, stoppingToken);
    }

    private async Task PersistBatchAsync(List<Payment> batch, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPaymentsRepository>();

        for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            try
            {
                _logger.LogInformation($"Saving batch of {batch.Count} payments");
                await repository.CreatePaymentAsync(batch, cancellationToken);
                return;
            }
            catch (Exception)
            {
                _logger.LogError($"Error saving batch {batch.Count} payments");
            }
        }
    }
}