using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Response;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Presentation.Worker;

public class ProcessPaymentWorkerService : BackgroundService
{
    private readonly IChannelQueueService<PaymentContract> _queueToProcess;
    private readonly IChannelQueueService<Payment> _queueProcessed;
    private readonly ILogger<ProcessPaymentWorkerService> _logger;
    private readonly IPaymentProcessor _defaultProcessor;
    private readonly IPaymentProcessor _fallbackProcessor;
    private readonly IMemoryCache _memoryCache;
    private const string CACHE_KEY_DEFAULT = "DefaultHealthCheck";
    private const string CACHE_KEY_FALLBACK = "FallbackHealthCheck";
    private const int MAX_RETRIES = 3;

    public ProcessPaymentWorkerService(
        IChannelQueueService<PaymentContract> queueToProcess,
        IChannelQueueService<Payment> queueProcessed,
        ILogger<ProcessPaymentWorkerService> logger,
        PaymentProcessorFactory paymentProcessorFactory,
        IMemoryCache memoryCache)
    {
        _queueToProcess = queueToProcess;
        _queueProcessed = queueProcessed;
        _logger = logger;
        _defaultProcessor = paymentProcessorFactory.CreateFactory(PaymentProcessorEnum.DEFAULT);
        _fallbackProcessor = paymentProcessorFactory.CreateFactory(PaymentProcessorEnum.FALLBACK);
        _memoryCache = memoryCache;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workerCount = 10;
        var workers = new List<Task>();
        for (int i = 0; i < workerCount; i++)
            workers.Add(ProcessQueueAsync(stoppingToken));

        await Task.WhenAll(workers);
    }

    private async Task ProcessQueueAsync(CancellationToken stoppingToken)
    {
        await foreach (var paymentContract in _queueToProcess.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation($"Starting payment processing {paymentContract.CorrelationId}");
                await ProcessPaymentAsync(paymentContract, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment {paymentContract.CorrelationId}");
            }
        }
    }

    private async Task ProcessPaymentAsync(PaymentContract paymentContract, CancellationToken cancellationToken)
    {
        var payment = new Payment(paymentContract.CorrelationId, paymentContract.Amount, DateTime.UtcNow, PaymentProcessorEnum.NONE);
        for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            var (processor, bestProcessor) = GetBetterServicePaymentProcessing();
            var requestedAt = DateTime.UtcNow;
            var paymentProcessorRequest = new PaymentProcessorContract(payment.CorrelationId, payment.Amount, requestedAt);

            var response = await processor.PaymentProcessorAsync(paymentProcessorRequest);
            if (response.IsSuccess)
            {
                await _queueProcessed.EnqueueAsync(payment with { RequestedAt = requestedAt, ProcessedBy = bestProcessor }, cancellationToken);
                return;
            }
            else
            {
                (processor, bestProcessor) = bestProcessor == PaymentProcessorEnum.DEFAULT
                    ? (_fallbackProcessor, PaymentProcessorEnum.FALLBACK)
                    : (_defaultProcessor, PaymentProcessorEnum.DEFAULT);
                var secondServiceResponse = await processor.PaymentProcessorAsync(paymentProcessorRequest);
                if (secondServiceResponse.IsSuccess)
                {
                    await _queueProcessed.EnqueueAsync(payment with { RequestedAt = requestedAt, ProcessedBy = bestProcessor }, cancellationToken);
                    return;
                }
            }

            if (attempt == MAX_RETRIES)
                throw new ApplicationException("Unable to process payment on any service.");

            await Task.Delay(100 * attempt, cancellationToken);
        }
    }

    private (IPaymentProcessor paymentProcessor, PaymentProcessorEnum paymentProcessorType) GetBetterServicePaymentProcessing()
    {
        var defaultHealthCheck = _memoryCache.Get<PaymentProcessorHealthResponse>(CACHE_KEY_DEFAULT)!;
        var fallbackHealthCheck = _memoryCache.Get<PaymentProcessorHealthResponse>(CACHE_KEY_FALLBACK)!;
        var diff = Math.Abs(defaultHealthCheck.MinResponseTime - fallbackHealthCheck.MinResponseTime);

        if (!defaultHealthCheck.Failing && diff <= 200)
            return (_defaultProcessor, PaymentProcessorEnum.DEFAULT);
        
        if (defaultHealthCheck.MinResponseTime > fallbackHealthCheck.MinResponseTime)
            return (_fallbackProcessor, PaymentProcessorEnum.FALLBACK);

        return (_defaultProcessor, PaymentProcessorEnum.DEFAULT);
    }
}