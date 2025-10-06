using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Application.Interface;

namespace RinhaBackend.Api.Worker;

public class PaymentFallbackHealthCheckWorkerService
    : BackgroundService
{
    private const string CACHE_KEY = "PaymentProcessorFallbackIsFailing";
    private const int TIME_IN_SECONDS_FOR_EACH_REQUEST = 5;
    private readonly IPaymentProcessorFallbackApiService _paymentProcessorFallbackApiService;
    private readonly IMemoryCache _memoryCache;

    public PaymentFallbackHealthCheckWorkerService(
        IPaymentProcessorFallbackApiService paymentProcessorFallbackApiService,
        IMemoryCache memoryCache)
    {
        _paymentProcessorFallbackApiService = paymentProcessorFallbackApiService;
        _memoryCache = memoryCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _paymentProcessorFallbackApiService.PaymentProcessorHealthCheck();

            _memoryCache.Set(CACHE_KEY, !response.IsSuccess ? true : response.Value?.Failing);
            await Task.Delay(TimeSpan.FromSeconds(TIME_IN_SECONDS_FOR_EACH_REQUEST), stoppingToken);
        }
    }
}