using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Presentation.Worker;

public class PaymentHealthCheckWorkerService
    : BackgroundService
{
    private const string CACHE_KEY_DEFAULT = "PaymentProcessorDefaultIsFailing";
    private const string CACHE_KEY_FALLBACK = "PaymentProcessorFallbackIsFailing";
    private const int TIME_IN_SECONDS_FOR_EACH_REQUEST = 5;
    private readonly PaymentProcessorFactory _paymentProcessorFactory;
    private readonly IMemoryCache _memoryCache;

    public PaymentHealthCheckWorkerService(
        PaymentProcessorFactory paymentProcessorFactory,
        IMemoryCache memoryCache)
    {
        _paymentProcessorFactory = paymentProcessorFactory;
        _memoryCache = memoryCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PaymentsHealthCheck(PaymentProcessorEnum.DEFAULT);
            await PaymentsHealthCheck(PaymentProcessorEnum.FALLBACK);
            await Task.Delay(TimeSpan.FromSeconds(TIME_IN_SECONDS_FOR_EACH_REQUEST), stoppingToken);
        }
    }

    private async Task PaymentsHealthCheck(PaymentProcessorEnum paymentProcessorType)
    {
        var processor = _paymentProcessorFactory.CreateFactory(paymentProcessorType);
        var response = await processor.PaymentProcessorHealthCheck();
        _memoryCache.Set(
            paymentProcessorType == PaymentProcessorEnum.DEFAULT ? CACHE_KEY_DEFAULT : CACHE_KEY_FALLBACK,
            !response.IsSuccess ? true : response.Value?.Failing);
    }
}