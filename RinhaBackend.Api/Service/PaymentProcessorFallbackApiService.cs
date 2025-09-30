using RinhaBackend.Api.Exceptions;
using RinhaBackend.Api.Interface;
using RinhaBackend.Api.MediatR;
using RinhaBackend.Api.MediatR.Request;
using RinhaBackend.Api.MediatR.Response;
using RinhaBackend.Api.Service.Api;

namespace RinhaBackend.Api.Service;

public class PaymentProcessorFallbackApiService
    : IPaymentProcessorFallbackApiService
{
    private readonly IPaymentProcessorFallbackApi _paymentProcessorFallbackApi;

    public PaymentProcessorFallbackApiService(IPaymentProcessorFallbackApi paymentProcessorFallbackApi)
        => _paymentProcessorFallbackApi = paymentProcessorFallbackApi;

    public async Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request)
    {
        var response = await _paymentProcessorFallbackApi.PaymentProcessorAsync(request);
        return !response.IsSuccessful
            ? Result<PaymentProcessorResponse>.Fail(new PaymentProcessorException($"Payment processor fallback error! {response.Error.Message}"))
            : Result<PaymentProcessorResponse>.Ok(response.Content);
    }

    public async Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck()
    {
        var response = await _paymentProcessorFallbackApi.PaymentProcessorHealthCheck();
        return !response.IsSuccessful
            ? Result<PaymentProcessorHealthResponse>.Fail(new PaymentProcessorException($"Payment health fallback error! {response.Error.Message}"))
            : Result<PaymentProcessorHealthResponse>.Ok(response.Content);
    }
}