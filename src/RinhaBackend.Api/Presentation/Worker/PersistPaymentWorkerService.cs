using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Domain.Entity;

namespace RinhaBackend.Api.Presentation.Worker;

public class PersistPaymentWorkerService : BackgroundService
{
    private readonly IChannelQueueService<Payment> _queue;
    private readonly ILogger<PersistPaymentWorkerService> _logger;
    private readonly IRedisService _cache;

    public PersistPaymentWorkerService(
        IChannelQueueService<Payment> queue,
        ILogger<PersistPaymentWorkerService> logger,
        IRedisService cache)
    {
        _queue = queue;
        _logger = logger;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var payment in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation($"Saving payment {payment.CorrelationId}");
                await _cache.AddPaymentAsync(payment);
                return;
            }
            catch (Exception)
            {
                _logger.LogError($"Error saving payment {payment.CorrelationId}");
            }
        }
    }
}