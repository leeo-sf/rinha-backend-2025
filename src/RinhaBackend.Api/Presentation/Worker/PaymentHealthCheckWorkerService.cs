using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Presentation.Worker;

public class PaymentHealthCheckWorkerService
    : BackgroundService
{
    private const string CACHE_KEY_DEFAULT = "DefaultHealthCheck";
    private const string CACHE_KEY_FALLBACK = "FallbackHealthCheck";
    private const int TIME_IN_SECONDS_FOR_EACH_REQUEST = 5;
    private readonly IPaymentProcessor _defaultProcessor;
    private readonly IPaymentProcessor _fallbackProcessor;
    private readonly IMemoryCache _memoryCache;

    public PaymentHealthCheckWorkerService(
        PaymentProcessorFactory paymentProcessorFactory,
        IMemoryCache memoryCache)
    {
        _defaultProcessor = paymentProcessorFactory.CreateFactory(PaymentProcessorEnum.DEFAULT);
        _fallbackProcessor = paymentProcessorFactory.CreateFactory(PaymentProcessorEnum.FALLBACK);
        _memoryCache = memoryCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.WhenAll(
                PaymentsHealthCheck(_defaultProcessor, PaymentProcessorEnum.DEFAULT),
                PaymentsHealthCheck(_fallbackProcessor, PaymentProcessorEnum.FALLBACK));
            await Task.Delay(TimeSpan.FromSeconds(TIME_IN_SECONDS_FOR_EACH_REQUEST), stoppingToken);
        }
    }

    private async Task PaymentsHealthCheck(IPaymentProcessor processor, PaymentProcessorEnum paymentProcessorType)
    {
        var response = await processor.PaymentProcessorHealthCheck();
        _memoryCache.Set(
            paymentProcessorType == PaymentProcessorEnum.DEFAULT ? CACHE_KEY_DEFAULT : CACHE_KEY_FALLBACK,
            response.Value);
    }
}