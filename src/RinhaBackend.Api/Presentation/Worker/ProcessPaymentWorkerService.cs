using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Response;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Enum;
using RinhaBackend.Api.Domain.Interface;

namespace RinhaBackend.Api.Presentation.Worker;

public class ProcessPaymentWorkerService : BackgroundService
{
    private readonly IPaymentQueueService _queue;
    private readonly ILogger<ProcessPaymentWorkerService> _logger;
    private readonly IPaymentProcessor _defaultProcessor;
    private readonly IPaymentProcessor _fallbackProcessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _memoryCache;
    private const string CACHE_KEY_DEFAULT = "DefaultHealthCheck";
    private const string CACHE_KEY_FALLBACK = "FallbackHealthCheck";
    private const int MAX_RETRIES = 3;
    private PaymentProcessorHealthResponse _defaultHealthCheck = default!;
    private PaymentProcessorHealthResponse _fallbackHealthCheck = default!;

    public ProcessPaymentWorkerService(
        IPaymentQueueService queue,
        ILogger<ProcessPaymentWorkerService> logger,
        PaymentProcessorFactory paymentProcessorFactory,
        IServiceProvider serviceProvider,
        IMemoryCache memoryCache)
    {
        _queue = queue;
        _logger = logger;
        _defaultProcessor = paymentProcessorFactory.CreateFactory(PaymentProcessorEnum.DEFAULT);
        _fallbackProcessor = paymentProcessorFactory.CreateFactory(PaymentProcessorEnum.FALLBACK);
        _serviceProvider = serviceProvider;
        _memoryCache = memoryCache;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workerCount = 8;
        var workers = new List<Task>();
        for (int i = 0; i < workerCount; i++)
            workers.Add(Task.Run(async () => await ProcessQueueAsync(stoppingToken), stoppingToken));

        await Task.WhenAll(workers);
    }

    private async Task ProcessQueueAsync(CancellationToken stoppingToken)
    {
        await foreach (var paymentContract in _queue.ReadAllAsync(stoppingToken))
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
        SetDefaultProcessorAsFailing();
        var payment = new Payment(paymentContract.CorrelationId, paymentContract.Amount, DateTime.UtcNow, PaymentProcessorEnum.NONE);
        for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            var (processor, bestProcessor) = GetBetterServicePaymentProcessing();
            var requestedAt = DateTime.UtcNow;
            var paymentProcessorRequest = new PaymentProcessorContract(payment.CorrelationId, payment.Amount, requestedAt);

            var response = await processor.PaymentProcessorAsync(paymentProcessorRequest);
            if (response.IsSuccess)
            {
                await InsertProcessedPayment(payment with { RequestedAt = requestedAt, ProcessedBy = bestProcessor }, cancellationToken);
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
                    await InsertProcessedPayment(payment with { RequestedAt = requestedAt, ProcessedBy = bestProcessor }, cancellationToken);
                    return;
                }
            }

            if (attempt == MAX_RETRIES)
                throw new ApplicationException("Unable to process payment on any service.");

            await Task.Delay(50 * attempt, cancellationToken);
        }
    }

    private (IPaymentProcessor paymentProcessor, PaymentProcessorEnum paymentProcessorType) GetBetterServicePaymentProcessing()
        => !_defaultHealthCheck.Failing && _defaultHealthCheck.MinResponseTime <= _fallbackHealthCheck.MinResponseTime
            ? (_defaultProcessor, PaymentProcessorEnum.DEFAULT)
            : (_fallbackProcessor, PaymentProcessorEnum.FALLBACK);

    private async Task InsertProcessedPayment(Payment payment, CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentsRepository>();
            await paymentRepository.CreatePaymentAsync(payment, cancellationToken);
        }
    }

    private void SetDefaultProcessorAsFailing()
    {
        _defaultHealthCheck = _memoryCache.Get<PaymentProcessorHealthResponse>(CACHE_KEY_DEFAULT)!;
        _fallbackHealthCheck = _memoryCache.Get<PaymentProcessorHealthResponse>(CACHE_KEY_FALLBACK)!;
    }
}