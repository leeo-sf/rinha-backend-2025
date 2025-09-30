
using Microsoft.Extensions.Caching.Memory;
using RinhaBackend.Api.Interface;

namespace RinhaBackend.Api.Worker;

public class PaymentDefaultHealthCheckWorkerService
    : BackgroundService
{
    private const string CACHE_KEY = "PaymentProcessorDefaultIsFailing";
    private const int TIME_IN_SECONDS_FOR_EACH_REQUEST = 5;
    private readonly IPaymentProcessorDefaultApiService _paymentProcessorDefaultApiService;
    private readonly IMemoryCache _memoryCache;

    public PaymentDefaultHealthCheckWorkerService(
        IPaymentProcessorDefaultApiService paymentProcessorDefaultApiService,
        IMemoryCache memoryCache)
    {
        _paymentProcessorDefaultApiService = paymentProcessorDefaultApiService;
        _memoryCache = memoryCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _paymentProcessorDefaultApiService.PaymentProcessorHealthCheck();

            _memoryCache.Set(CACHE_KEY, !response.IsSuccess ? true : response.Value?.Failing);
            await Task.Delay(TimeSpan.FromSeconds(TIME_IN_SECONDS_FOR_EACH_REQUEST), stoppingToken);
        }
    }
}