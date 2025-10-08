using RinhaBackend.Api.Application.Exceptions;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Application.Response;
using RinhaBackend.Api.Application.Service.Api;

namespace RinhaBackend.Api.Application.Service;

public class FallbackPaymentProcessor
    : IPaymentProcessor
{
    private readonly IPaymentProcessorFallbackApi _paymentProcessorFallbackApi;

    public FallbackPaymentProcessor(IPaymentProcessorFallbackApi paymentProcessorFallbackApi)
        => _paymentProcessorFallbackApi = paymentProcessorFallbackApi;

    public async Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck()
    {
        var response = await _paymentProcessorFallbackApi.PaymentProcessorHealthCheck();
        return !response.IsSuccessful
            ? Result<PaymentProcessorHealthResponse>.Fail(new PaymentProcessorException($"Payment health fallback error! {response.Error.Message}"))
            : Result<PaymentProcessorHealthResponse>.Ok(response.Content);
    }

    public async Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request)
    {
        var response = await _paymentProcessorFallbackApi.PaymentProcessorAsync(request);
        return !response.IsSuccessful
            ? Result<PaymentProcessorResponse>.Fail(new PaymentProcessorException($"Payment processor fallback error! {response.Error.Message}"))
            : Result<PaymentProcessorResponse>.Ok(response.Content);
    }
}