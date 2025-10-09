using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Application;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Application.Response;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Enum;
using RinhaBackend.Api.Domain.Interface;
using System.Threading.Channels;

namespace RinhaBackend.Api.Presentation.Worker;

public class ProcessPaymentWorkerService : BackgroundService
{
    private readonly Channel<PaymentContract> _channel;
    private readonly ILogger<ProcessPaymentWorkerService> _logger;
    private readonly PaymentProcessorFactory _paymentProcessorFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMemoryCache _memoryCache;
    private const string CACHE_KEY_DEFAULT = "PaymentProcessorDefaultIsFailing";
    private const int MAX_RETRIES = 3;

    public ProcessPaymentWorkerService(
        Channel<PaymentContract> channel,
        ILogger<ProcessPaymentWorkerService> logger,
        PaymentProcessorFactory paymentProcessorFactory,
        IServiceScopeFactory serviceScopeFactory,
        IMemoryCache memoryCache)
    {
        _channel = channel;
        _logger = logger;
        _paymentProcessorFactory = paymentProcessorFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _memoryCache = memoryCache;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var paymentContract in _channel.Reader.ReadAllAsync(stoppingToken))
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
        for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            var requestedAt = DateTime.UtcNow;
            var payment = new Payment(paymentContract.CorrelationId, paymentContract.Amount, requestedAt, PaymentProcessorEnum.NONE);

            if (_memoryCache.Get<bool>(CACHE_KEY_DEFAULT))
            {
                var responseFallback = await SendPaymentProcessing(new(payment.CorrelationId, payment.Amount, payment.RequestedAt!), PaymentProcessorEnum.FALLBACK);
                if (responseFallback.IsSuccess)
                {
                    await InsertProcessedPayment(payment with { ProcessedBy = PaymentProcessorEnum.FALLBACK });
                    return;
                }
            }

            var responseDefault = await SendPaymentProcessing(new(payment.CorrelationId, payment.Amount, payment.RequestedAt!), PaymentProcessorEnum.DEFAULT);
            if (!responseDefault.IsSuccess)
            {
                await InsertProcessedPayment(payment with { ProcessedBy = PaymentProcessorEnum.DEFAULT });
                return;
            }

            if (attempt == MAX_RETRIES)
                throw new ApplicationException("Unable to process payment on any service.");

            await Task.Delay(100 * attempt, cancellationToken);
        }
    }

    private async Task<Result<PaymentProcessorResponse>> SendPaymentProcessing(
        PaymentProcessorContract request,
        PaymentProcessorEnum paymentProcessorType)
    {
        var processor = _paymentProcessorFactory.CreateFactory(paymentProcessorType);
        return await processor.PaymentProcessorAsync(request);
    }

    private async Task InsertProcessedPayment(Payment payment)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentsRepository>();
            await paymentRepository.CreatePaymentAsync(payment, CancellationToken.None);
        }
    }
}